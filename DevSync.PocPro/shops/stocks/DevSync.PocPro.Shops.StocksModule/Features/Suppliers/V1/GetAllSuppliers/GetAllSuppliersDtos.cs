namespace DevSync.PocPro.Shops.StocksModule.Features.Suppliers.V1.GetAllSuppliers;

public record GetAllSuppliersRequest(
    string Keyword = "",
    int Page = 1,
    int PageSize = 20,
    string SortBy = "created-dsc");