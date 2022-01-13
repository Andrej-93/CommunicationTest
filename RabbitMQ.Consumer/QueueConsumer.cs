using Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Consumer
{
    public static class QueueConsumer
    {
        public static void Consume(IModel channel, IMessageRepository? messageRepository)
        {
            channel.QueueDeclare("communication-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var data = JsonSerializer.Deserialize<string>((message));
                Console.WriteLine("I recived the message - " + data);
                Console.WriteLine("Writing to the database...");

                messageRepository?.Add(new Message { MessageString = data ?? string.Empty });
            };

            channel.BasicConsume("communication-queue", true, consumer);
        }
    }
}