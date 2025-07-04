namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.AddApplicationUser;

public class AddApplicationUserEndpoint(
    IApplicationDbContext applicationDbContext, 
    IHttpContextAccessor httpContextAccessor,
    IIdentityServices identityServices) 
    : Endpoint<AddApplicationUserRequest, BaseResponse<AddApplicationUserResponse>>
{
    public override void Configure()
    {
        Post("/api/v1/accounts/users");
        Description(x => x
            .WithName("AddUser")
            .Produces<BaseResponse<AddApplicationUserResponse>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
        );
    }

    public override async Task HandleAsync(AddApplicationUserRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var newUserId = string.Empty;
        
        if (!req.TenantAccount)
        {
            var loggedInUser = await applicationDbContext
                .ApplicationUsers
                .Include(a => a.Permissions)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.UserId == userId, ct);
            if (loggedInUser == null)
            {
                await SendNotFoundAsync(cancellation: ct);
                return;
            }
            
            if (!loggedInUser.HasPermission(PermissionType.MANAGE_USERS))
            {
                await SendForbiddenAsync(cancellation: ct);
                return;
            }
            
            try
            {
               newUserId = await identityServices.RegisterUserLoginAsync(req.Username, req.Email!, req.Password!,
                    httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault()!);
            }
            catch (Exception)
            {
                await SendErrorsAsync(StatusCodes.Status500InternalServerError, cancellation: ct);
            }
        }
        
        var existingUser = await applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(a => a.UserId == req.Username, ct);

        if (existingUser != null)
        {
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }
        
        List<Permission> permissions = [];
        
        var photoUrl = string.Empty;
        if (req.PhotoFile != null)
        {
            //TODO - to be implemented
        }
        
        if (req.PermissionTypes != null && req.PermissionTypes.Any())
        {
            var permissionTypeEnums = req.PermissionTypes
                .Select(Enum.Parse<PermissionType>)
                .ToList();

            permissions = await applicationDbContext.Permissions
                .Where(p => permissionTypeEnums.Contains(p.PermissionType))
                .ToListAsync(ct);
        }

        var result = ApplicationUser.Create(req.FirstName, req.LastName, req.OtherNames ?? string.Empty, req.Email ?? string.Empty, newUserId, photoUrl,
            permissions, TenantId.Of(req.TenantId));
        
        await applicationDbContext.ApplicationUsers.AddAsync(result, ct);
        await applicationDbContext.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetApplicationUserDetailsEndpoint>(new
            {
                result.Id,
            },
            new BaseResponse<AddApplicationUserResponse>("Application created successfully", true)
            {
                Data = new AddApplicationUserResponse(result.Id.Value)
            }, cancellation: ct);
    }
}

public class AddApplicationUserRequestValidator : Validator<AddApplicationUserRequest>
{
    public AddApplicationUserRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name cannot be empty.").NotNull();
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name cannot be empty.").NotNull();
        
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email)).WithMessage("Email should be a valid email address").NotNull();
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .When(x => !x.TenantAccount).WithMessage("Password cannot be empty.").NotNull();
        
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username cannot be empty.").NotNull();
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("Tenant id cannot be empty.").NotNull();
        
        RuleFor(x => x.PermissionTypes)
            .Must(list => list == null || list.All(pt => Enum.TryParse<PermissionType>(pt, out _)))
            .When(x => x.PermissionTypes != null && x.PermissionTypes.Any())
            .WithMessage("All permission types must be valid PermissionType enum values.");
    }
}