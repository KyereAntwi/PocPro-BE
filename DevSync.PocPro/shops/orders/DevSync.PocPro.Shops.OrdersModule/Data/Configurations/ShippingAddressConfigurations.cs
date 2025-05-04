namespace DevSync.PocPro.Shops.OrdersModule.Data.Configurations;

public class ShippingAddressConfigurations : IEntityTypeConfiguration<ShippingAddress>
{
    public void Configure(EntityTypeBuilder<ShippingAddress> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, dbId => ShippingAddressId.Of(dbId));
        
        builder.Property(x => x.OrderId)
            .HasConversion(id => id.Value, dbId => OrderId.Of(dbId))
            .IsRequired();

        builder.Property(x => x.ContactName)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.Address1)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.ContactPhone)
            .HasMaxLength(15)
            .IsRequired();
        
        builder.Property(t => t.Region)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<Region>(dbType!));
    }
}