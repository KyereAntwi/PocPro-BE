namespace DevSync.PocPro.Shops.GeneralSettings.Features.V1.GetSettings;

public class GetSettingsEndpoint(IGeneralSettingsModuleDbContext generalSettingsModuleDbContext)
    : EndpointWithoutRequest<BaseResponse<SettingResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/settings");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = await generalSettingsModuleDbContext
            .Settings
            .Select(s => new SettingResponse
            {
                LogoUrl = s.LogoUrl ?? string.Empty,
                CompanyName = s.CompanyName,
                Description = s.GeneralSiteDescription ?? string.Empty,
                Contact = new ContactResponse(
                    s.Email ?? string.Empty, 
                    s.PhoneNumber ?? string.Empty, 
                    s.Address ?? string.Empty),
                Seo = s.Seo != null ? 
                    new SeoResponse(
                        s.Seo.MetaTitle, 
                        s.Seo.MetaDescription, 
                        s.Seo.MetaKeyword) : 
                    null,
                SocialLinks = s
                    .SocialLinks
                    .Select(sl => 
                        new SocialLinkResponse(
                            sl.Name ?? string.Empty, 
                            sl.Url ?? string.Empty))
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (query is null)
        {
            await SendAsync(
                response: new BaseResponse<SettingResponse>("Failed to fetch settings information", false)
                {
                    Errors = ["Settings information not set"]
                },
                statusCode: StatusCodes.Status404NotFound, ct);
            return;
        }

        await SendOkAsync(new BaseResponse<SettingResponse>("Settings fetched successfully", true)
        {
            Data = query
        }, ct);
    }
}