namespace DevSync.PocPro.Shops.PointOfSales.Data.Configurations;

public class PointOfSaleConfigurations : IEntityTypeConfiguration<PointOfSale>
{
    public void Configure(EntityTypeBuilder<PointOfSale> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(posId => posId.Value, dbId => PointOfSaleId.Of(dbId));
        
        builder
            .HasMany(v => v.Sessions)
            .WithOne()
            .HasForeignKey(f => f.PointOfSaleId);

        builder.Property(x => x.Email).HasMaxLength(255);
        builder.Property(x => x.Phone).HasMaxLength(15);
        builder.Property(x => x.Address).HasMaxLength(200);
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}