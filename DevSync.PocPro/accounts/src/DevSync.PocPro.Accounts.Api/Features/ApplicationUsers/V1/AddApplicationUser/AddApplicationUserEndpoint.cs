namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.AddApplicationUser;

public class AddApplicationUserEndpoint(
    IApplicationDbContext applicationDbContext, 
    IHttpContextAccessor httpContextAccessor,
    IPublishEndpoint publishEndpoint,
    ILogger<AddApplicationUserEndpoint> logger) 
    : Endpoint<AddApplicationUserRequest, BaseResponse<AddApplicationUserResponse>>
{
    public override void Configure()
    {
        Post("api/v1/accounts/users");
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
        
        var accountAppHeader = httpContextAccessor.HttpContext!.Request.Headers["X-ACCOUNT-APP"].FirstOrDefault();
        if (accountAppHeader == null)
        {
            var loggedInUser = await applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(a => a.UserId == userId, ct);
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
        }
        
        var photoUrl = string.Empty;
        Permission[] permissions = [];
        
        if (req.PhotoFile != null)
        {
            //TODO - to be implemented
        }
        if (req.PermissionTypes != null && req.PermissionTypes.Any())
        {
            //TODO - to be implemented
        }
        
        var result = ApplicationUser.Create(req.FirstName, req.LastName, req.OtherNames, req.Email, userId!, photoUrl, permissions, TenantId.Of(req.TenantId));
        
        await applicationDbContext.ApplicationUsers.AddAsync(result, ct);
        await applicationDbContext.SaveChangesAsync(ct);

        try
        {
            await publishEndpoint.Publish(new RegisterUserLoginEvent
            {
                Username = req.Username,
                Email = req.Email,
            }, ct);
        }
        catch (Exception e)
        {
            logger.LogError("Error occured sending user registration event. Error = {Message}", e.Message);
        }

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
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email should be a valid email address").NotNull();
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username cannot be empty.").NotNull();
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("Tenant id cannot be empty.").NotNull();
    }
}