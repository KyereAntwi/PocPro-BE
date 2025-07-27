namespace DevSync.PocPro.Shops.GeneralSettings.Interfaces;

public interface IGeneralSettingsModuleDbContext
{
    DbSet<Settings> Settings { get; }
    DbSet<SeoInformation> SeoInformation { get; }
    DbSet<SocialLink> SocialLinks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}