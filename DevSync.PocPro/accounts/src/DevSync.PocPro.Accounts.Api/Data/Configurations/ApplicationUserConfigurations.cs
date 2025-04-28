namespace DevSync.PocPro.Accounts.Api.Data.Configurations;

public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, dbId => ApplicationUserId.Of(dbId));
        
        builder.Property(a => a.TenantId).HasConversion(a => a.Value, dbId => TenantId.Of(dbId));
        
        builder
            .HasMany(t => t.Permissions)
            .WithMany();
    }
}