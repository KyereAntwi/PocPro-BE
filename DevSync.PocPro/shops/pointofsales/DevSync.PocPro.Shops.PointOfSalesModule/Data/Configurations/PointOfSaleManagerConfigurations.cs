using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.PointOfSales.Data.Configurations;

public class PointOfSaleManagerConfigurations : IEntityTypeConfiguration<PointOfSaleManager>
{
    public void Configure(EntityTypeBuilder<PointOfSaleManager> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(managerId => managerId.Value, dbId => PosManagerId.Of(dbId));
        
        builder.Property(a => a.PointOfSaleId).HasConversion(a => a.Value, dbId => PointOfSaleId.Of(dbId));

        builder.Property(a => a.UserId).IsRequired();
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}