namespace DevSync.PocPro.Shops.Shared.Events;

public record OrderCanceledEvent : IntegrationEvents
{
    public IEnumerable<ProductCanceledDto> Items { get; set; } = new List<ProductCanceledDto>();
    public string TenantIdentifier { get; init; } = string.Empty;
}

public record ProductCanceledDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; set; }
    public Guid PosId { get; set; }
}