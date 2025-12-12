using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CleanArch_Products.Application.Messaging;
using Confluent.Kafka;

namespace CleanArch_Products.Infra.Utils.Messaging
{
    public class KafkaMessageBus : IMessageBus
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaMessageBus(string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            
            var payload = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<Null, string> {Value = payload});

        }
    }
}