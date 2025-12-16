using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using CleanArch_Products.Application.Messaging;

namespace CleanArch_Products.Infra.Utils.Messaging
{
    public class SQSMessageBus : IMessageBus
    {

        private readonly AmazonSQSClient _sqsClient;
        private readonly string _queueName;

        public SQSMessageBus(string serviceURL, string queueName, string region, string accessKey = null, string secretKey = null)
        {
            var config = new AmazonSQSConfig
            {
                ServiceURL = serviceURL,
                AuthenticationRegion = region

            };

            var client = new AmazonSQSClient(accessKey, secretKey, config);

            _sqsClient = client;
            _queueName = queueName;
        }

        public async Task PublishAsync<T>(string topic, T message)
        {

            var payload = System.Text.Json.JsonSerializer.Serialize(message);

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = (await _sqsClient.GetQueueUrlAsync(_queueName)).QueueUrl,
                MessageBody = payload
            };

            try
            {
                await _sqsClient.SendMessageAsync(sendMessageRequest);
            }
            catch (AmazonSQSException ex)
            {
                throw ex;
            }
        }
    }
}