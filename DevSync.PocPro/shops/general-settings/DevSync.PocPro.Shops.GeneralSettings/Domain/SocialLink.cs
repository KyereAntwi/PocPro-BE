namespace DevSync.PocPro.Shops.GeneralSettings.Domain;

public class SocialLink : BaseEntity<Guid>
{
    public string? Name { get; set; }
    public string? Url { get; set; }
    
    public Guid SettingsId { get; set; }
    public Settings? Settings { get; set; }
}