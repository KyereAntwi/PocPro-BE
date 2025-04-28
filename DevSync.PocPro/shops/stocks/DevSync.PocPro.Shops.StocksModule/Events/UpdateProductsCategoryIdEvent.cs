using DevSync.PocPro.Shared.Domain.Events;

namespace DevSync.PocPro.Shops.StocksModule.Events;

public record UpdateProductsCategoryIdEvent : IntegrationEvents
{
    public Guid CategoryId { get; set; }
}