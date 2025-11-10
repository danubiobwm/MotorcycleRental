using System.Threading.Tasks;

namespace Infra.Messaging
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(string routingKey, T @event);
    }
}
