namespace DevSync.PocPro.Shops.StocksModule.Data.Configurations;

public class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(vehicleId => vehicleId.Value, dbId => ProductId.Of(dbId));
        
        builder.Property(x => x.CategoryId)
            .HasConversion(categoryId => categoryId.Value, dbId => CategoryId.Of(dbId));
        
        builder.Property(x => x.BrandId)
            .IsRequired(false)
            .HasConversion(brandId => brandId!.Value, dbId => BrandId.Of(dbId));
        
        builder
            .HasMany(v => v.Stocks)
            .WithOne()
            .HasForeignKey(f => f.ProductId);
        
        builder
            .HasMany(v => v.Media)
            .WithOne()
            .HasForeignKey(f => f.ProductId);
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}