namespace DevSync.PocPro.Shops.StocksModule.Data.Configurations;

public class ContactConfigurations : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(vehicleId => vehicleId.Value, dbId => ContactId.Of(dbId));
        
        builder.Property(a => a.SupplierId).HasConversion(a => a.Value, dbId => SupplierId.Of(dbId));
        
        builder.Property(t => t.ContactType)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<ContactType>(dbType!));
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}