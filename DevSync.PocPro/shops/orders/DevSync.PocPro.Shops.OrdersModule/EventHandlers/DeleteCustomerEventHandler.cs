using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DevSync.PocPro.Shops.OrdersModule.EventHandlers;

public class DeleteCustomerEventHandler: BackgroundService
{
    private readonly IOrderModuleDbContext _ordersModuleDbContext;
    private readonly ILogger<DeleteCustomerEventHandler> _logger;
    private readonly RabbitMqConnectionFactory _factory;
    private readonly string _queueName = "procpro_shop";
    private IChannel _channel;
    private IConnection _connection;

    public DeleteCustomerEventHandler(
        IOrderModuleDbContext ordersModuleDbContext,
        ILogger<DeleteCustomerEventHandler> logger,
        RabbitMqConnectionFactory factory)
    {
        _ordersModuleDbContext = ordersModuleDbContext;
        _factory = factory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = await _factory.CreateConnection();
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        
        await _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: stoppingToken);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                var eventObj = JsonSerializer.Deserialize<DeleteCustomerEvent>(message);
                if (eventObj == null)
                    throw new InvalidOperationException("Deserialized event is null.");
                
                _logger.LogInformation("Received event: {@Event}", eventObj);
                
                await _ordersModuleDbContext
                    .Orders
                    .Where(order => order.CustomerId == CustomerId.Of(eventObj.CustomerId))
                    .ExecuteUpdateAsync(o => 
                        o.SetProperty(order => order.CustomerId, CustomerId.Of(Guid.Empty)), cancellationToken: stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        };
    }
    
    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}