using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace DevSync.PocPro.Identity.Events;

public class RegisterUserLoginEventHandler (ILogger<RegisterUserLoginEventHandler> logger, UserManager<IdentityUser> userManager)
    : IConsumer<RegisterUserLoginEvent>
{
    public async Task Consume(ConsumeContext<RegisterUserLoginEvent> context)
    {
        logger.LogInformation("Integrated Event handled: {IntegrationEvent}. Occured on: {OccuredOn}", context.Message.GetType().Name, context.Message.OccuredOn);

        var user = new IdentityUser
        {
            UserName = context.Message.Username,
            Email = context.Message.Email ?? string.Empty,
        };
        
        var result = await userManager.CreateAsync(user, context.Message.Password);

        if (!result.Succeeded)
        {
            logger.LogError("Error creating a user with username {Username}", context.Message.Username);
        }
    }
}