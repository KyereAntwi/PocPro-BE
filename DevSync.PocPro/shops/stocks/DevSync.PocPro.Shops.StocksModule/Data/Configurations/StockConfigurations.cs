namespace DevSync.PocPro.Shops.StocksModule.Data.Configurations;

public class StockConfigurations : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(vehicleId => vehicleId.Value, dbId => StockId.Of(dbId));
        builder.Property(a => a.ProductId).HasConversion(a => a.Value, dbId => ProductId.Of(dbId));
    }
}