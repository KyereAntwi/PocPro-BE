namespace DevSync.PocPro.Shops.OrdersModule.EventHandlers;

public class DeleteCustomerEventHandler(OrdersModuleDbContext ordersModuleDbContext, ILogger<DeleteCustomerEventHandler> logger) 
    : IConsumer<DeleteCustomerEvent>
{
    public async Task Consume(ConsumeContext<DeleteCustomerEvent> context)
    {
        logger.LogInformation("Integrated Event handled: {IntegrationEvent}. Occured on: {OccuredOn}", context.Message.GetType().Name, context.Message.OccuredOn);

        await ordersModuleDbContext
            .Orders
            .Where(order => order.CustomerId == CustomerId.Of(context.Message.CustomerId))
            .ExecuteUpdateAsync(o => 
                o.SetProperty(order => order.CustomerId, CustomerId.Of(Guid.Empty)));
    }
}