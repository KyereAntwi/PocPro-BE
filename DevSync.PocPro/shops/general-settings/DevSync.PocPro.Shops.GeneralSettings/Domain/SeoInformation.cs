namespace DevSync.PocPro.Shops.GeneralSettings.Domain;

public class SeoInformation : BaseEntity<Guid>
{
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string MetaKeyword { get; set; } = string.Empty;

    public Guid SettingsId { get; set; }
    public Settings? Settings { get; set; }
}