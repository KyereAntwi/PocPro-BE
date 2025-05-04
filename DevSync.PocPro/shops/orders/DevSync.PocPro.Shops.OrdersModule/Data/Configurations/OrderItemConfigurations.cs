namespace DevSync.PocPro.Shops.OrdersModule.Data.Configurations;

public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, dbId => OrderItemId.Of(dbId));
        
        builder.Property(x => x.OrderId)
            .HasConversion(id => id.Value, dbId => OrderId.Of(dbId))
            .IsRequired();
    }
}