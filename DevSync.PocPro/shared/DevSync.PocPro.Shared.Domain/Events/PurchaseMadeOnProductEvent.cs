namespace DevSync.PocPro.Shared.Domain.Events;

public record PurchaseMadeOnProductEvent : IntegrationEvents
{
    public IEnumerable<PurchaseMadeOnProductDto> Items { get; set; } = new List<PurchaseMadeOnProductDto>();
    public string TenantIdentifier { get; set; } = string.Empty;
}

public record PurchaseMadeOnProductDto(Guid ProductId, int QuantityPurchased);