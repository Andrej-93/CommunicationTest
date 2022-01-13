// See https://aka.ms/new-console-template for more information
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Consumer;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddTransient<IMessageRepository, MessageRepository>()
        .AddTransient<AppDbContext>(s => new AppDbContext("Server=communicationdb;Database=CommunicationDb;User Id=sa; Password=Pwd12345!;")))
    //.AddDbContext<AppDbContext>(options => options
    //              .UseSqlServer("Server=localhost;Database=CommunicationDb;User Id=sa; Password=Pwd12345!;"), ServiceLifetime.Singleton))
    .Build();

RunApplication(host);
await host.RunAsync();

static void RunApplication(IHost host, int? retry = 0)
{
    int retryForAvailability = retry.Value;
    try
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@rabbitmq:5672")
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var messageRepository = host.Services.GetService<IMessageRepository>();
        Console.WriteLine("Communication started!");

        QueueConsumer.Consume(channel, messageRepository);
    }
    catch (Exception)
    {
        if (retryForAvailability < 20)
        {
            retryForAvailability++;
            Console.WriteLine("RabbitMQ is not running yet, trying in 1 second... retrys left: " + (10 - retryForAvailability));
            Thread.Sleep(1000);
            RunApplication(host, retryForAvailability);
        }
    }

    Console.ReadLine();
}