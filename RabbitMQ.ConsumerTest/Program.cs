using Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddTransient<IMessageRepository, MessageRepository>()
        .AddTransient(s => new AppDbContext(config.GetConnectionString("Default"))))
    .Build();

RunApplication(host, config.GetValue<string>("EventBusSettings:HostAddress"));
await host.RunAsync();

static void RunApplication(IHost host, string uriSetting, int? retry = 0)
{
    int retryForAvailability = retry.Value;
    try
    {
        var messageRepository = host.Services.GetService<IMessageRepository>();
        Console.WriteLine("Communication started!");

        //QueueConsumer.Consume(uriSetting, messageRepository);

        var factory = new ConnectionFactory
        {
            Uri = new Uri(uriSetting)
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

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
            Console.WriteLine("I recived the message - " + data + Environment.NewLine + "Writing to the database...");
            messageRepository?.Add(new Message { MessageString = data ?? string.Empty });
        };

        channel.BasicConsume("communication-queue", true, consumer);
    }
    catch (Exception ex)
    {
        if (retryForAvailability < 20)
        {
            Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            retryForAvailability++;
            Console.WriteLine("RabbitMQ is not running yet, trying in 1 second... retrys left: " + (20 - retryForAvailability));
            Thread.Sleep(1000);
            RunApplication(host, uriSetting, retryForAvailability);
        }
    }

    Console.ReadLine();
}