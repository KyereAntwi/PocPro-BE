using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace DevSync.PocPro.Shared.Domain.Utils;

public class EventPublisher
{
    private readonly RabbitMqConnectionFactory _factory;
    private IConnection _connection;

    public EventPublisher(RabbitMqConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task PublishAsync<T>(T eventMessage, string exchange, string routingKey, CancellationToken cancellation = default)
    {
        _connection = await _factory
            .CreateConnection();
        
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellation);
        //await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, durable: true, autoDelete: false, cancellationToken: cancellation);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventMessage));
        await channel.BasicPublishAsync(exchange, routingKey, body: body, cancellationToken: cancellation);
    }
}