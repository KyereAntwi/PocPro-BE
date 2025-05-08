namespace DevSync.PocPro.Shops.PrivateCustomers.Data.Configurations;

public class CustomerConfigurations : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, dbId => CustomerId.Of(dbId));

        builder.Property(c => c.FullName).IsRequired();
    }
}