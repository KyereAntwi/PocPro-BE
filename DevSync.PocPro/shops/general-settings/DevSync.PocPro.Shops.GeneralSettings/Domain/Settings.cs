namespace DevSync.PocPro.Shops.GeneralSettings.Domain;

public class Settings : BaseEntity<Guid>
{
    public string? LogoUrl { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? GeneralSiteDescription { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public SeoInformation? Seo { get; set; }
    public ICollection<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
}