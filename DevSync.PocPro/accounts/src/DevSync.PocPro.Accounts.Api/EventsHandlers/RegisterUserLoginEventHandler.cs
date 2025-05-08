namespace DevSync.PocPro.Accounts.Api.EventsHandlers;

public abstract class RegisterUserLoginEventHandler(
    ILogger<RegisterUserLoginEventHandler> logger,
    IKeycloakServices keycloakServices) 
    : IConsumer<RegisterUserLoginEvent>
{
    public async Task Consume(ConsumeContext<RegisterUserLoginEvent> context)
    {
        logger.LogInformation("Integrated Event handled: {IntegrationEvent}. Occured on: {OccuredOn}", context.Message.GetType().Name, context.Message.OccuredOn);
        await keycloakServices.RegisterUserLoginAsync(context.Message.Username, context.Message.Username);
    }
}