namespace DevSync.PocPro.Accounts.Api.Utils;

public class TenantDatabaseSettings
{
    public string Server { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string MasterConnectionString { get; set; } = string.Empty;
}