namespace DevSync.PocPro.Persistence.Data.TenantManagement.Configurations;

public class TenantsConfigurations : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(tenantId => tenantId.Value, dbId => TenantId.Of(dbId));
        
        builder
            .HasMany(t => t.ApplicationUsers)
            .WithOne()
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}