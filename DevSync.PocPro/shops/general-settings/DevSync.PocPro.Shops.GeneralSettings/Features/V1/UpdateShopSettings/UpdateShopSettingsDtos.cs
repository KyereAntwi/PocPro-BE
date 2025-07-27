namespace DevSync.PocPro.Shops.GeneralSettings.Features.V1.UpdateShopSettings;

public record UpdateShopSettingsRequest
{
    public string? LogoUrl { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ContactDto? Contact { get; set; }
    public IEnumerable<SocialLinkDto>? SocialLinks { get; set; } = new List<SocialLinkDto>();
    public SeoInformationDto? Seo { get; set; }
}

public record ContactDto(string Phone, string Email, string Address);
public record SocialLinkDto(string Name, string Url);
public record SeoInformationDto(string MetaTitle, string MetaDescription, string MetaKeyword);