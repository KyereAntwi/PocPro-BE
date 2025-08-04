namespace DevSync.PocPro.Shops.Api.EventHandlers;

public class AddProductToQueryEventHandler : BackgroundService
{
    private readonly RabbitMqConnectionFactory _factory;
    private readonly string _queueName = "add_products_to_query";
    private IChannel _channel;
    private IConnection _connection;
    private ILogger<AddProductToQueryEventHandler> _logger;
    private IDocumentStore _documentStore;
    
    public AddProductToQueryEventHandler(
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
        await _channel.QueueBindAsync(_queueName, "shop_exchange", "add_products_to_query", cancellationToken: stoppingToken);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                var eventObj = JsonSerializer.Deserialize<AddProductToQueryEvent>(message);
                if (eventObj == null)
                    throw new InvalidOperationException("Deserialized event is null.");

                _logger.LogInformation("Received event: {@Event}", eventObj);
                
                if (string.IsNullOrWhiteSpace(eventObj.TenantIdentifier))
                {
                    _logger.LogError("Tenant identifier was not provided");
                    throw new ArgumentNullException(nameof(eventObj.TenantIdentifier));
                }
                
                await using var session = _documentStore.LightweightSession(eventObj.TenantIdentifier);
        
                var existingProduct = session
                    .Query<ProductsQueryModule.Models.Product>()
                    .FirstOrDefault(p => p.Identifier == eventObj.ProductId && p.PosId == eventObj.PosId);
                
                if (existingProduct is null)
                {
                    var newProduct = new ProductsQueryModule.Models.Product
                    {
                        Identifier = eventObj.ProductId,
                        Name = eventObj.Name,
                        PhotoUrl = eventObj.PhotoUrl ?? string.Empty,
                        CurrentPrice = eventObj.CurrentPrice,
                        Keywords = eventObj.Keywords,
                        IsFeatured = eventObj.IsFeatured,
                        QuantityLeft = eventObj.QuantityLeft,
                        PosId = eventObj.PosId,
                        Brand = !string.IsNullOrWhiteSpace(eventObj.BrandTitle)
                            ? new ProductsQueryModule.Models.Brand
                            {
                                Id = eventObj.BrandId,
                                Title = eventObj.BrandTitle
                            }
                            : null,
                        Category = new ProductsQueryModule.Models.Category
                        {
                            Id = eventObj.CategoryId,
                            Title = eventObj.CategoryTitle  
                        },
                        OnlineEnabled = eventObj.IsOnline,
                        BarcodeNumber = eventObj.BarcodeNumber,
                        Ratings = eventObj.Ratings
                    };

                    session.Store(newProduct);
                }
                else
                {
                    existingProduct.CurrentPrice = eventObj.CurrentPrice;
                    existingProduct.QuantityLeft = eventObj.QuantityLeft;
            
                    session.Update(existingProduct);
                }
        
                await session.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", ex.Message ?? "Unknown");
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
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