using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CleanArch_Products.Application.Messaging;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace CleanArch_Products.Infra.Utils.Messaging
{
    public class KafkaMessageBus : IMessageBus, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaMessageBus> _logger;
        private readonly IAsyncPolicy _policy;


        public KafkaMessageBus(string bootstrapServers, ILogger<KafkaMessageBus> logger)
        {

            _logger = logger;
            
            var config = new ProducerConfig 
            { 
                BootstrapServers = bootstrapServers,
                  // Kafka-level delivery guarantees
                Acks = Acks.All,                        // wait for all ISR replicas (strongest guarantee)
                EnableIdempotence = true,               // producer-side dedup (requires Acks.All)
                MaxInFlight = 5,                        // max unacknowledged requests (with idempotence, safe up to 5)
                MessageSendMaxRetries = 5,              // Kafka internal retries
                RetryBackoffMs = 100,
                RetryBackoffMaxMs = 2000,
                LingerMs = 5,                            // batch messages for up to 5ms
                CompressionType = CompressionType.Snappy // compress messages to save bandwidth

            
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _policy = BuildPolicy();

        }

        private IAsyncPolicy BuildPolicy()
        {
             // Circuit Breaker: open after 5 failures within a 30s window, stay open for 30s
            var circuitBreaker = Policy
                .Handle<KafkaException>(e => !e.Error.IsFatal)
                .Or<ProduceException<Null, string>>(e => !e.Error.IsFatal)
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (ex, breakDuration) =>
                        _logger.LogError("Kafka circuit breaker OPEN for {Duration}s. Error: {Msg}",
                            breakDuration.TotalSeconds, ex.Message),
                    onReset: () =>
                        _logger.LogInformation("Kafka circuit breaker CLOSED — broker recovered"),
                    onHalfOpen: () =>
                        _logger.LogInformation("Kafka circuit breaker HALF-OPEN — probing broker"));

            // Retry: 5 attempts with exponential backoff + jitter
            var retry = Policy
                .Handle<KafkaException>(e => !e.Error.IsFatal)
                .Or<ProduceException<Null, string>>(e => !e.Error.IsFatal)
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: attempt =>
                    {
                        // Exponential backoff: 1s, 2s, 4s, 8s, 16s + jitter
                        var exponential = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                        var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500));
                        return exponential + jitter;
                    },
                    onRetry: (exception, timeSpan, retryCount, _) =>
                        _logger.LogWarning(
                            "Kafka publish retry {RetryCount}/5 after {Delay:F1}s. Error: {Msg}",
                            retryCount, timeSpan.TotalSeconds, exception.Message));

            // Wrap: retry is the outer policy, circuit breaker is inner
            // This means: retry calls happen through the circuit breaker
            // If the breaker opens, retries stop immediately
            return Policy.WrapAsync(retry, circuitBreaker);
        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            var payload = JsonSerializer.Serialize(message);

            try
            {
                await _policy.ExecuteAsync(async () =>
                {
                    var result = await _producer.ProduceAsync(
                        topic,
                        new Message<Null, string> { Value = payload });

                    if (result.Status != PersistenceStatus.Persisted)
                    {
                        throw new KafkaException(
                            new Error(ErrorCode.Local_MsgTimedOut,
                                $"Message not persisted. Status: {result.Status}"));
                    }

                    _logger.LogInformation("Kafka message delivered to {Offset}", result.TopicPartitionOffset);
                });
            }
            catch (BrokenCircuitException ex)
            {
                // Circuit is open — this is where you'd write to the Outbox instead
                _logger.LogError(ex, "Kafka circuit is OPEN. Message for topic '{Topic}' could not be delivered.", topic);
                throw new InvalidOperationException(
                    $"Message broker is currently unavailable (circuit open). Topic: {topic}", ex);
            }
        }

        public void Dispose() => _producer?.Dispose();
    }
}