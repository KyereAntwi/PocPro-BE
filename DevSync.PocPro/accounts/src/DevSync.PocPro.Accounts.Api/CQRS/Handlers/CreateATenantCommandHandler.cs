namespace DevSync.PocPro.Accounts.Api.CQRS.Handlers;

public class CreateATenantCommandHandler(
    IApplicationDbContext applicationDbContext, 
    ILogger<CreateATenantEndpoint> logger,
    RabbitMqConnectionFactory rabbitMqConnection,
    TenantDatabaseSettings tenantDatabaseSettings)
    : Shared.Domain.CQRS.ICommandHandler<CreateATenantRequest, CreateATenantResponse>
{
    public async Task<Result<CreateATenantResponse>> HandleAsync(CreateATenantRequest command, CancellationToken cancellationToken = default)
    {
        var existingUniqueIdentifier = await applicationDbContext
            .Tenants
            .Where(t => t.UniqueIdentifier.ToLower() == command.UniqueIdentifier.ToLower())
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingUniqueIdentifier != null)
        {
            return Result.Fail("A tenant with the same unique identifier already exists.");
        }
        
        var connectionString = $"Host={tenantDatabaseSettings.Server};Port={tenantDatabaseSettings.Port};Database={command.UniqueIdentifier};Username={tenantDatabaseSettings.Username};Password={tenantDatabaseSettings.Password};SSL Mode=Require;Trust Server Certificate=true;";
        _ = Enum.TryParse<SubscriptionType>(command.SubscriptionType, out var subscription);
        var tenant = Tenant.Create(command.UniqueIdentifier, connectionString, subscription);
        
        await applicationDbContext.Tenants.AddAsync(tenant, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        try
        {
            var integrationEvent = new GenerateTenantDatabaseEvent
            {
                TenantId = tenant.Id.Value,
                DatabaseName = tenant.UniqueIdentifier,
                ConnectionString = connectionString
            };
            var publisher = new EventPublisher(rabbitMqConnection);
            await publisher.PublishAsync(integrationEvent, "shop_exchange", "generate_database", cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError("Error publishing integration event {Error}", e.Message);
        }
        
        return Result.Ok(new CreateATenantResponse(tenant.Id.Value));
    }
}