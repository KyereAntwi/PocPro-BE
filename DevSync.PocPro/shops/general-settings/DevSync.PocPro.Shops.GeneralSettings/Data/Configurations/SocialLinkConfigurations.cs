namespace DevSync.PocPro.Shops.GeneralSettings.Data.Configurations;

public class SocialLinkConfigurations : IEntityTypeConfiguration<SocialLink>
{
    public void Configure(EntityTypeBuilder<SocialLink> builder)
    {
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}