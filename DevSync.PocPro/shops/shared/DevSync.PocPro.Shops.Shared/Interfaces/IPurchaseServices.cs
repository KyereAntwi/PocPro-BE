using DevSync.PocPro.Shops.Shared.Dtos;
using FluentResults;

namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IPurchaseServices
{
    Task<Result> MakePurchaseOnProducts(IEnumerable<MakePurchaseOnProductsRequest> requests, CancellationToken cancellationToken = default);
}