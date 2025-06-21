namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Category : BaseEntity<CategoryId>
{
    public static Category Create(string title, string? description, string? imageUrl)
    {
        var category = new Category
        {
            Id = CategoryId.Of(Guid.CreateVersion7()),
            Title = title,
            Description = description,
            ImageUrl = imageUrl
        };
        
        return category;
    }
    
    public void Update(string title, string? description, StatusType? status, string? imageUrl)
    {
        Title = title;
        Description = description;
        Status = status;
        ImageUrl = imageUrl;
    }

    public string Title { get; private set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; private set; }
}