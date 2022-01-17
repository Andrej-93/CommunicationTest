using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CommunicationTest.Data
{
    public static class QueueProducer
    {
        public static void Publish(string message, string uri = "amqp://guest:guest@localhost:5672")
        {
            var factory = new ConnectionFactory() { Uri = new Uri(uri) };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("communication-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish("", "communication-queue", null, body);
        }
    }
}