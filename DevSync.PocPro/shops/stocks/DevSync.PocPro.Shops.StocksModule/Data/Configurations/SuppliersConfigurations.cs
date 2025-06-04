namespace DevSync.PocPro.Shops.StocksModule.Data.Configurations;

public class SuppliersConfigurations : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(vehicleId => vehicleId.Value, dbId => SupplierId.Of(dbId));
        
        builder
            .HasMany(v => v.Contacts)
            .WithOne()
            .HasForeignKey(f => f.SupplierId);
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}