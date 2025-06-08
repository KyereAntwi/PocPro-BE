namespace DevSync.PocPro.Identity.Events;

public record RegisterUserLoginEvent : IntegratedEvents
{
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
}