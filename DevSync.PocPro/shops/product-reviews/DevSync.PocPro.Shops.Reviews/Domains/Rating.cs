namespace DevSync.PocPro.Shops.Reviews.Domains;

public class Rating : BaseEntity<Guid>
{
    public ProductId ProductId { get; set; } = default!;
    public float StarRating { get; set; }
}