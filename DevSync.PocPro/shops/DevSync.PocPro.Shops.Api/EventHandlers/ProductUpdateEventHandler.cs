namespace DevSync.PocPro.Shops.Api.EventHandlers;

public class ProductUpdateEventHandler : BackgroundService
{
    private readonly RabbitMqConnectionFactory _factory;
    private readonly string _queueName = "update_product";
    private IChannel _channel;
    private IConnection _connection;
    private ILogger<AddProductToQueryEventHandler> _logger;
    private IDocumentStore _documentStore;
    
    public ProductUpdateEventHandler(
        ILogger<AddProductToQueryEventHandler> logger,
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
        await _channel.QueueBindAsync(_queueName, "shop_exchange", "update_product", cancellationToken: stoppingToken);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var eventObj = JsonSerializer.Deserialize<ProductUpdatedEvent>(message);
                if (eventObj == null)
                    throw new InvalidOperationException("Deserialized event is null.");

                _logger.LogInformation("Received event: {@Event}", eventObj);

                await using var session = _documentStore.LightweightSession(eventObj.Identifier);

                var existingProduct = session
                    .Query<ProductsQueryModule.Models.Product>()
                    .FirstOrDefault(p => p.Identifier == eventObj.ProductId);

                if (existingProduct is null)
                {
                    return;
                }

                existingProduct.Name = eventObj.Name;
                existingProduct.BarcodeNumber = eventObj.BarcodeNumber;
                existingProduct.PhotoUrl = eventObj.PhotoUrl;
                existingProduct.IsFeatured = eventObj.IsFeatured;

                session.Update(existingProduct);
                await session.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Product rated event processed for ProductId: {ProductId}", eventObj.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SendProductRatedEvent");
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