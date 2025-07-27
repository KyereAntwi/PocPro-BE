namespace DevSync.PocPro.Shops.GeneralSettings.Features.V1.UpdateShopSettings;

public class UpdateShopSettingsEndpoint(
    IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices, IGeneralSettingsModuleDbContext settingsModuleDbContext) 
    : Endpoint<UpdateShopSettingsRequest>
{
    public override void Configure()
    {
        Post("/api/v1/settings");
    }

    public override async Task HandleAsync(UpdateShopSettingsRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_SETTINGS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendAsync(new BaseResponse<Guid>("Permission Denied", false)
                {
                    Errors = ["You do not have required permission"]
                }, 
                StatusCodes.Status403Forbidden, ct);
            return;
        }
        
        var settings = await settingsModuleDbContext
            .Settings
            .Include(s => s.Seo)
            .Include(s => s.SocialLinks)
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (settings is not null)
        {
            settings.CompanyName = req.CompanyName;
            settings.LogoUrl = req.LogoUrl ?? settings.LogoUrl ?? string.Empty;
            settings.GeneralSiteDescription = req.Description ?? settings.GeneralSiteDescription ?? string.Empty;
            settings.Email = req.Contact?.Email ?? settings.Email ?? string.Empty;
            settings.PhoneNumber = req.Contact?.Phone ?? settings.PhoneNumber ?? string.Empty;
            settings.Address = req.Contact?.Address ?? settings.Address ?? string.Empty;

            if (req.Seo != null)
            {
                settings.Seo!.MetaTitle = req.Seo.MetaTitle;
                settings.Seo.MetaKeyword = req.Seo.MetaKeyword;
                settings.Seo.MetaDescription = req.Seo.MetaDescription;
            }

            if (req.SocialLinks!.Any())
            {
                UpdateSocialLink(req, settings);
            }

            settingsModuleDbContext.Settings.Update(settings);
        }
        else
        {
            var newSettings = new Settings()
            {
                LogoUrl = req.LogoUrl ?? string.Empty,
                CompanyName = req.CompanyName,
                GeneralSiteDescription = req.Description ?? string.Empty,
                Email = req.Contact?.Email ?? string.Empty,
                PhoneNumber = req.Contact?.Phone ?? string.Empty,
                Address = req.Contact?.Address ?? string.Empty,
                Status = StatusType.Active
            };

            if (req.Seo != null)
            {
                CreateSeoInformation(req, newSettings);
            }

            if (req.SocialLinks!.Any())
            {
                CreateSocialLinks(req, newSettings);
            }

            await settingsModuleDbContext.Settings.AddAsync(newSettings, ct);
        }

        await settingsModuleDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }

    private static void CreateSeoInformation(UpdateShopSettingsRequest req, Settings newSettings)
    {
        newSettings.Seo = new SeoInformation
        {
            MetaTitle = req.Seo!.MetaTitle,
            MetaKeyword = req.Seo.MetaKeyword,
            MetaDescription = req.Seo.MetaDescription,
            Status = StatusType.Active
        };
    }

    private static void CreateSocialLinks(UpdateShopSettingsRequest req, Settings newSettings)
    {
        newSettings.SocialLinks = req
            .SocialLinks?
            .Select(s => new SocialLink
            {
                Name = s.Name,
                Url = s.Url,
                Status = StatusType.Active
            }).ToList()!;
    }

    private static void UpdateSocialLink(UpdateShopSettingsRequest req, Settings existingSettings)
    {
        foreach (var link in req.SocialLinks!)
        {
            var existingLink = existingSettings.SocialLinks!.FirstOrDefault(s => s.Name == link.Name);

            if (existingLink is null)
            {
                var newLink = new SocialLink
                {
                    Name = link.Name,
                    Url = link.Url
                };
                        
                existingSettings.SocialLinks.Add(newLink);
            }
            else
            {
                existingLink.Url = link.Url;
            }
        }
    }
}

public class UpdateShopSettingsRequestValidator : Validator<UpdateShopSettingsRequest>
{
    public UpdateShopSettingsRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .NotNull();

        RuleFor(x => x.Contact)
            .NotNull().WithMessage("Contact information should not be null");

        RuleFor(x => x.Seo!.MetaTitle)
            .NotEmpty().WithMessage("Metal title is required")
            .NotNull()
            .When(x => x.Seo is not null);
        RuleFor(x => x.Seo!.MetaDescription)
            .NotEmpty().WithMessage("Metal description is required")
            .NotNull()
            .When(x => x.Seo is not null);
        RuleFor(x => x.Seo!.MetaKeyword)
            .NotEmpty().WithMessage("Metal keyword is required")
            .NotNull()
            .When(x => x.Seo is not null);
        
        RuleFor(x => x.SocialLinks)
            .Must(list => list!.All(item => !string.IsNullOrWhiteSpace(item.Name) && !string.IsNullOrWhiteSpace(item.Url)))
            .WithMessage("All social links must have a name and url")
            .When(x => x.SocialLinks!.Any());
    }
}