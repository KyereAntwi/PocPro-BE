namespace DevSync.PocPro.Shops.GeneralSettings.Features.V1.GetSettings;

public record SettingResponse
{
    public string? LogoUrl { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ContactResponse Contact { get; set; } = default!;
    public IEnumerable<SocialLinkResponse> SocialLinks { get; set; } = new List<SocialLinkResponse>();
    public SeoResponse? Seo { get; set; }
}

public record ContactResponse(
    string Email,
    string Phone,
    string Address);

public record SocialLinkResponse(
    string Name,
    string Url);

public record SeoResponse(
    string MetaTitle,
    string MetaDescription,
    string MetaKeyword);