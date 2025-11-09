using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;




namespace Infra.Messaging
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(string queue, object message);
    }

    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher()
        {
            _factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
        }

        public Task PublishAsync(string queue, object message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue, durable: false, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish("", queue, null, body);
            return Task.CompletedTask;
        }
    }
}
