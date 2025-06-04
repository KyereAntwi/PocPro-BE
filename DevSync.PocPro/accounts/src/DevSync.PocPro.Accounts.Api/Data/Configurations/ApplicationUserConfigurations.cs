namespace DevSync.PocPro.Accounts.Api.Data.Configurations;

public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, dbId => ApplicationUserId.Of(dbId));
        
        builder.Property(a => a.TenantId).HasConversion(a => a!.Value, dbId => TenantId.Of(dbId));
        builder.HasIndex(a => a.TenantId);
        
        builder
            .HasMany(t => t.Permissions)
            .WithMany();
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}