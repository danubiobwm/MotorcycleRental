using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Infra.Messaging
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly ConnectionFactory _factory;
        public RabbitMqPublisher(IConfiguration cfg)
        {
            var host = cfg["RabbitMq__Host"] ?? "localhost";
            _factory = new ConnectionFactory() { HostName = host };
        }

        public Task PublishAsync<T>(string routingKey, T @event)
        {
            using var conn = _factory.CreateConnection();
            using var channel = conn.CreateModel();
            channel.ExchangeDeclare(exchange: "motorcycle_exchange", type: ExchangeType.Fanout, durable: true);
            var body = JsonSerializer.SerializeToUtf8Bytes(@event);
            channel.BasicPublish(exchange: "motorcycle_exchange", routingKey: routingKey, basicProperties: null, body: body);
            return Task.CompletedTask;
        }
    }
}
