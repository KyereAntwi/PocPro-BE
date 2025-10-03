namespace DevSync.PocPro.Shops.Api.EventHandlers;

public class GenerateTenantDatabaseEventHandler 
    : BackgroundService
{
    private readonly ITenantRegistrationServices _tenantRegistrationServices;
    private readonly ILogger<GenerateTenantDatabaseEventHandler> _logger;
    private readonly RabbitMqConnectionFactory _factory;
    private readonly string _queueName = "generate_database";
    private IChannel _channel;
    private IConnection _connection;

    public GenerateTenantDatabaseEventHandler(
        ITenantRegistrationServices tenantRegistrationServices,
        ILogger<GenerateTenantDatabaseEventHandler> logger,
        RabbitMqConnectionFactory factory)
    {
        _tenantRegistrationServices = tenantRegistrationServices;
        _logger = logger;
        _factory = factory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = await _factory.CreateConnection();
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        
        await _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: stoppingToken);
        await _channel.QueueBindAsync(_queueName, "shop_exchange", "generate_database", cancellationToken: stoppingToken);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                var eventObj = JsonSerializer.Deserialize<GenerateTenantDatabaseEvent>(message);
                if (eventObj == null)
                    throw new InvalidOperationException("Deserialized event is null.");

                _logger.LogInformation("Received event: {@Event}", eventObj);

                await _tenantRegistrationServices.GenerateTenantDatabase(eventObj.DatabaseName, stoppingToken);
                await _tenantRegistrationServices.ApplyMigrationAsync(eventObj.ConnectionString, stoppingToken);
                await _tenantRegistrationServices.DeployTenantFrontendAsync(eventObj.DatabaseName, stoppingToken);
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