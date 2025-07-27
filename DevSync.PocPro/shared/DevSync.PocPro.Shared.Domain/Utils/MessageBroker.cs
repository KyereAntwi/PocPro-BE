namespace DevSync.PocPro.Shared.Domain.Utils;

public class MessageBroker
{
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
}