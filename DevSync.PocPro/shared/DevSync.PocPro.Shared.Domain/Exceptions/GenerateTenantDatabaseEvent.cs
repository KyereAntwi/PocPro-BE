namespace DevSync.PocPro.Shared.Domain.Exceptions;

public record GenerateTenantDatabaseEvent : IntegrationEvents
{
    public Guid TenantId { get; init; }
    public string DatabaseName { get; init; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}