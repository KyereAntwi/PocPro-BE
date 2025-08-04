namespace DevSync.PocPro.Shops.Shared.Events;

public record ProductUpdatedEvent : IntegrationEvents
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BarcodeNumber { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public string Identifier { get; set; } = string.Empty;
}