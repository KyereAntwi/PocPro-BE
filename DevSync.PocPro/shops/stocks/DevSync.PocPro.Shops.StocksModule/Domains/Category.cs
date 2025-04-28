namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Category : BaseEntity<CategoryId>
{
    public static Category Create(string title)
    {
        var category = new Category
        {
            Title = title
        };
        
        return category;
    }
    
    public void Update(string title)
    {
        Title = title;
    }

    public string Title { get; private set; }
}