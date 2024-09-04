using Confluent.Kafka;

namespace PermissionsApi.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducerService()
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public virtual async Task ProduceMessageAsync(string topic, string key, string value)
        {
            try
            {
                var message = new Message<string, string> { Key = key, Value = value };
                var deliveryResult = await _producer.ProduceAsync(topic, message);
                Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}