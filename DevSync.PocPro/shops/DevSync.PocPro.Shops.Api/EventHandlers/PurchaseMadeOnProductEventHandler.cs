namespace DevSync.PocPro.Shops.Api.EventHandlers;

public class PurchaseMadeOnProductEventHandler : BackgroundService
{
    private readonly ILogger<PurchaseMadeOnProductEventHandler> _logger;
    private readonly IDocumentStore _documentStore;
    private readonly RabbitMqConnectionFactory _factory;
    private readonly string _queueName = "purchase_changes_in_product_query";
    private IChannel _channel;
    private IConnection _connection;

    public PurchaseMadeOnProductEventHandler(
        ILogger<PurchaseMadeOnProductEventHandler> logger,
        IDocumentStore documentStore,
        RabbitMqConnectionFactory factory)
    {
        _logger = logger;
        _documentStore = documentStore;
        _logger = logger;
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
                
                var eventObj = JsonSerializer.Deserialize<PurchaseMadeOnProductEvent>(message);
                if (eventObj == null)
                    throw new InvalidOperationException("Deserialized event is null.");

                _logger.LogInformation("Received event: {@Event}", eventObj);
                
                if (string.IsNullOrWhiteSpace(eventObj.TenantIdentifier))
                {
                    _logger.LogWarning("Event handling failed due to missing tenant identifier");
                    return;
                }
        
                await using var session = _documentStore.LightweightSession(eventObj.TenantIdentifier);

                foreach (var item in eventObj.Items)
                {
                    var existingProduct = session
                        .Query<ProductsQueryModule.Models.Product>()
                        .FirstOrDefault(p => p.Identifier == item.ProductId);

                    if (existingProduct is null)
                    {
                        continue;
                    }

                    existingProduct.QuantityLeft -= item.QuantityPurchased;
                    session.Update(existingProduct);
                }
        
                await session.SaveChangesAsync(stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        };
        
        await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
    }
    
    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}