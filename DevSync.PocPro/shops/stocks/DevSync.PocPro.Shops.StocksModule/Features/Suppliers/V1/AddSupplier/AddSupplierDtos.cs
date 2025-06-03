namespace DevSync.PocPro.Shops.StocksModule.Features.Suppliers.V1.AddSupplier;

public record AddSupplierRequest(
    string Title,
    string? Email,
    IEnumerable<ContactRequest> Contacts);

public record ContactRequest(string Person, string Value, string ContactType);