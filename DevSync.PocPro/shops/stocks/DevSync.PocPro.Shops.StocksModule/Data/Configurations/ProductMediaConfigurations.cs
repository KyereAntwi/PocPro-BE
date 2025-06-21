namespace DevSync.PocPro.Shops.StocksModule.Data.Configurations;

public class ProductMediaConfigurations : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(mediaId => mediaId.Value, dbId => ProductMediaId.Of(dbId));
        
        builder.Property(x => x.MediaType)
            .HasConversion(x => ToString(),
                dbType => Enum.Parse<ProductMediaType>(dbType!));
    }
}