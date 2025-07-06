namespace DevSync.PocPro.Shops.UserWishlistModule.Data.Configurations;

public class WishListItemConfiguration : IEntityTypeConfiguration<WishListItem>
{
    public void Configure(EntityTypeBuilder<WishListItem> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasConversion(
                id => id.Value,
                dbId => WishListItemId.Of(dbId));
        
        builder.Property(w => w.UserId)
            .IsRequired()
            .HasMaxLength(225);
        
        builder.Property(w => w.ProductId)
            .IsRequired()
            .HasConversion(
                productId => productId.Value,
                dbId => ProductId.Of(dbId));
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}