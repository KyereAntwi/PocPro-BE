namespace DevSync.PocPro.Shops.OrdersModule.Data.Configurations;

public class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, dbId => OrderId.Of(dbId));
        
        builder
            .HasMany(v => v.OrderItems)
            .WithOne()
            .HasForeignKey(f => f.OrderId);
        
        builder
            .HasOne(v => v.ShippingAddress)
            .WithOne();
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<OrderStatus>(dbType!));
        
        builder.Property(t => t.PaymentMethod)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<PaymentMethod>(dbType!))
            .IsRequired();
        
        builder.Property(t => t.Type)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<OrderType>(dbType!))
            .IsRequired();
    }
}