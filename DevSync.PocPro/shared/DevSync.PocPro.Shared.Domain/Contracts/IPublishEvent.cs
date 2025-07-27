namespace DevSync.PocPro.Shared.Domain.Contracts;

public interface IPublishEvent
{
    Task PublishAsync<T>(T eventMessage, string exchange, string routingKey, CancellationToken cancellation = default);
}