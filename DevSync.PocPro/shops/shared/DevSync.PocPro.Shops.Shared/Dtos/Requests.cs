namespace DevSync.PocPro.Shops.Shared.Dtos;

public record MakePurchaseOnProductsRequest(Guid ProductId, int Quantity);