namespace DevSync.PocPro.Accounts.Api.Data.Configurations;

public class PermissionConfigurations : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, dbId => PermissionId.Of(dbId));
        
        builder.Property(t => t.PermissionType)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<PermissionType>(dbType));
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}