namespace DevSync.PocPro.Shops.StocksModule.Data.Configurations;

public class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(vehicleId => vehicleId.Value, dbId => ProductId.Of(dbId));
        
        builder
            .HasMany(v => v.Stocks)
            .WithOne()
            .HasForeignKey(f => f.ProductId);
    }
}