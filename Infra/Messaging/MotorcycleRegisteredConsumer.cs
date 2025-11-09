using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Domain.Entities;
using Infra.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infra.Messaging
{
    public class MotorcycleRegisteredConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConnectionFactory _factory;

        public MotorcycleRegisteredConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "guest", Password = "guest" };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "MotorcycleRegistered", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                MotorcycleRegisteredMessage? data = null;

                try
                {
                    data = JsonSerializer.Deserialize<MotorcycleRegisteredMessage>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch
                {
                    // log or ignore bad message
                }

                if (data != null && data.Year == 2024)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Notifications.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        Message = $"Motorcycle {data.Plate} of year 2024 registered.",
                        CreatedAt = DateTime.UtcNow
                    });
                    await db.SaveChangesAsync();
                }

                // ack
                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(queue: "MotorcycleRegistered", autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }
    }

    public record MotorcycleRegisteredMessage(Guid Id, int Year, string Model, string Plate);
}
