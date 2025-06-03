namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Category : BaseEntity<CategoryId>
{
    public static Category Create(string title, string? description)
    {
        var category = new Category
        {
            Id = CategoryId.Of(Guid.CreateVersion7()),
            Title = title,
            Description = description,
            Status = true
        };
        
        return category;
    }
    
    public void Update(string title, string? description, bool status)
    {
        Title = title;
        Description = description;
        Status = status;
    }

    public string Title { get; private set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
}