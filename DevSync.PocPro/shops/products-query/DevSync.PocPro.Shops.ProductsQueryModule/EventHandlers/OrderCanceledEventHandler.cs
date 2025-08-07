namespace DevSync.PocPro.Shops.ProductsQueryModule.EventHandlers;

public class OrderCanceledEventHandler : BackgroundService
{
    private readonly RabbitMqConnectionFactory _factory;
    private const string _queueName = "purchase_changes_in_product_query";
    private IChannel _channel;
    private IConnection _connection;
    private ILogger<OrderCanceledEventHandler> _logger;
    private IDocumentStore _documentStore;
    
    public OrderCanceledEventHandler(
        ILogger<OrderCanceledEventHandler> logger,
        IDocumentStore documentStore,
        RabbitMqConnectionFactory factory)
    {
        _logger = logger;
        _documentStore = documentStore;
        _factory = factory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = await _factory.CreateConnection();
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        
        await _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: stoppingToken);
        await _channel.QueueBindAsync(_queueName, "shop_exchange", "purchase_changes_in_product_query", cancellationToken: stoppingToken);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var eventObj = JsonSerializer.Deserialize<OrderCanceledEvent>(message);
                if (eventObj == null)
                    throw new InvalidOperationException("Deserialized event is null.");

                _logger.LogInformation("Received event: {@Event}", eventObj);

                await using var session = _documentStore.LightweightSession(eventObj.TenantIdentifier);

                foreach (var item in eventObj.Items)
                {
                    var existingProduct = session
                        .Query<Product>()
                        .FirstOrDefault(p => 
                            p.Identifier == item.ProductId &&
                            p.PosId == item.PosId);

                    if (existingProduct is null)
                    {
                        continue;
                    }

                    existingProduct.QuantityLeft += item.Quantity;
                    session.Update(existingProduct);
                }

                await session.SaveChangesAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Error processing order canceled event: {Error}", e.Message);
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false,
                    cancellationToken: stoppingToken);
            }
        };
        
        await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer,
            cancellationToken: stoppingToken);
    }
    
    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}