using DevSync.PocPro.Shops.Shared.Dtos;
using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.Shared.Events;

public record SendProductsWithLowStockNotificationEvent : IntegrationEvents
{
    public IEnumerable<ProductsForPosDto> ProductsForPos { get; set; } = new List<ProductsForPosDto>();
}

public record ProductsForPosDto
{
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    public PointOfSaleId PointOfSaleId { get; set; } = null!;
    public Guid TenantId { get; set; }
}