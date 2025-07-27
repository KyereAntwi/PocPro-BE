namespace DevSync.PocPro.Shared.Domain.Events;

public record AddProductToQueryEvent : IntegrationEvents
{
    public string TenantIdentifier { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public decimal CurrentPrice { get; set; }
    public int QuantityLeft { get; set; }
    public Guid BrandId { get; set; }
    public string? BrandTitle { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryTitle { get; set; }
    public string BarcodeNumber { get; set; }
    public bool IsFeatured { get; set; }
    public Guid PosId { get; set; }
    public bool IsOnline { get; set; }
    public List<string> Keywords { get; set; } = new();
}

public record BrandEventDto(Guid Id, string Title);
public record CategoryEventDto(Guid Id, string Title);