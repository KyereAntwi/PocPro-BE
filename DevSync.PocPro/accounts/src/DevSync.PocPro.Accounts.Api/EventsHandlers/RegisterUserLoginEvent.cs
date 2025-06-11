namespace DevSync.PocPro.Accounts.Api.EventsHandlers;

public record RegisterUserLoginEvent : IntegrationEvents
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}