namespace DevSync.PocPro.Shops.StocksModule.Data.Configurations;

public class BrandConfigurations : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(brandId => brandId.Value, dbId => BrandId.Of(dbId));
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
        
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(b => b.Description)
            .HasMaxLength(500)
            .IsRequired(false);
    }
}