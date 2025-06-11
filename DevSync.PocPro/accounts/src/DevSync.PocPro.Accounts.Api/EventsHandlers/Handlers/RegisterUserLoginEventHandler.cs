namespace DevSync.PocPro.Accounts.Api.EventsHandlers.Handlers;

public class RegisterUserLoginEventHandler(
    ILogger<RegisterUserLoginEventHandler> logger, IIdentityServices identityServices) 
    : IConsumer<RegisterUserLoginEvent>
{
    public async Task Consume(ConsumeContext<RegisterUserLoginEvent> context)
    {
        logger.LogInformation("Integrated Event handled: {IntegrationEvent}. Occured on: {OccuredOn}", context.Message.GetType().Name, context.Message.OccuredOn);
        await identityServices.RegisterUserLoginAsync(context.Message.Username, context.Message.Email, context.Message.Password, context.Message.AccessToken);
    }
}