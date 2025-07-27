namespace DevSync.PocPro.Shops.GeneralSettings.Data.Configurations;

public class SeoConfigurations : IEntityTypeConfiguration<SeoInformation>
{
    public void Configure(EntityTypeBuilder<SeoInformation> builder)
    {
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}