using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevSync.PocPro.Shops.Reviews.Data.Configurations;

public class ProductReviewConfigurations: IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder
            .Property(x => x.ProductId)
            .IsRequired()
            .HasConversion(productId => productId.Value, dbId => ProductId.Of(dbId));
    }
}