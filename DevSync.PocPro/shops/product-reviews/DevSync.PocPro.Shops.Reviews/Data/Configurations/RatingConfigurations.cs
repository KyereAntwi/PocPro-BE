using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevSync.PocPro.Shops.Reviews.Data.Configurations;

public class RatingConfigurations : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder
            .Property(x => x.ProductId)
            .IsRequired()
            .HasConversion(productId => productId.Value, dbId => ProductId.Of(dbId));
    }
}