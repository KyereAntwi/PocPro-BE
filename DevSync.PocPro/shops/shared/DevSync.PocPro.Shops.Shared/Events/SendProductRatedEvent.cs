namespace DevSync.PocPro.Shops.Shared.Events;

public record SendProductRatedEvent : IntegrationEvents
{
    public Guid ProductId { get; init; }
    public string Identifier { get; init; } = string.Empty;
    public float Ratings { get; set; }
}