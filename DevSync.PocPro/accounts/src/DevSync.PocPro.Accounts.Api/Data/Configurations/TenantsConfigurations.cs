namespace DevSync.PocPro.Accounts.Api.Data.Configurations;

public class TenantsConfigurations : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, dbId => TenantId.Of(dbId));
        
        builder.Property(t => t.SubscriptionType)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<SubscriptionType>(dbType!));
    }
}