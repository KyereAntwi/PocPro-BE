using DevSync.PocPro.Shared.Domain.Events;
using DevSync.PocPro.Shops.Api.Services;

namespace DevSync.PocPro.Shops.Api.EventHandlers;

public class GenerateTenantDatabaseEventHandler(
    ITenantServices tenantServices,
    ITenantRegistrationServices tenantRegistrationServices,
    ILogger<GenerateTenantDatabaseEventHandler> logger) 
    : IConsumer<GenerateTenantDatabaseEvent>
{
    public async Task Consume(ConsumeContext<GenerateTenantDatabaseEvent> context)
    {
        logger.LogInformation("Integrated Event handled: {IntegrationEvent}. Occured on: {OccuredOn}", context.Message.GetType().Name, context.Message.OccuredOn);
        
        await tenantRegistrationServices.GenerateTenantDatabase(context.Message.DatabaseName);
        await tenantRegistrationServices.ApplyMigrationAsync(context.Message.ConnectionString);
        
        //TODO - Post tenant setup notification and update tenant status
    }
}