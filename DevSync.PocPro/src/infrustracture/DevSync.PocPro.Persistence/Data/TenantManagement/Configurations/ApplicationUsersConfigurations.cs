namespace DevSync.PocPro.Persistence.Data.TenantManagement.Configurations;

public class ApplicationUsersConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, dbId => ApplicationUserId.Of(dbId));
    }
}