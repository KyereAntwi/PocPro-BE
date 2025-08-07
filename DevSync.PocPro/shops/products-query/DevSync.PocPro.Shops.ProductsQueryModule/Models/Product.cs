namespace DevSync.PocPro.Shops.ProductsQueryModule.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid Identifier { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public decimal CurrentPrice { get; set; }
    public Brand? Brand { get; set; }
    public Category? Category { get; set; }
    public bool IsFeatured { get; set; }
    public bool OnlineEnabled { get; set; }
    public int QuantityLeft { get; set; }
    public Guid PosId { get; set; }
    public string? BarcodeNumber { get; set; }
    public float Ratings { get; set; }
    public int PurchaseCount { get; set; } = 0;
    public List<string> Keywords { get; set; } = [];
}