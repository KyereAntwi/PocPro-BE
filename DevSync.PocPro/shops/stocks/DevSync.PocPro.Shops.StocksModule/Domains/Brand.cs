namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Brand : BaseEntity<BrandId>
{
    public static Brand Create(string title, string? description, string? imageUrl)
    {
        var brand = new Brand
        {
            Id = BrandId.Of(Guid.CreateVersion7()),
            Title = title,
            Description = description,
            ImageUrl = imageUrl
        };
        
        return brand;
    }
    
    public void Update(string title, string? description, StatusType? status, string? imageUrl)
    {
        this.Title = title;
        this.Description = description;
        this.Status = status;
        this.ImageUrl = imageUrl;
    }
    
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
}