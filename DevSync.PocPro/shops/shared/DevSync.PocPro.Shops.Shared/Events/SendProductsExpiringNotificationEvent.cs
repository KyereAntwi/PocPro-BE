namespace DevSync.PocPro.Shops.Shared.Events;

public record SendProductsExpiringNotificationEvent : IntegrationEvents
{
    public IEnumerable<ProductsForPosDto> ProductsForPos { get; set; } = new List<ProductsForPosDto>();
}