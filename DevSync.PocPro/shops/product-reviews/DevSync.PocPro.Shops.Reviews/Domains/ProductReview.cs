namespace DevSync.PocPro.Shops.Reviews.Domains;

public class ProductReview: BaseEntity<Guid>
{
    public ProductId ProductId { get; set; } = default!;
    public string Message { get; set; } = string.Empty;
}