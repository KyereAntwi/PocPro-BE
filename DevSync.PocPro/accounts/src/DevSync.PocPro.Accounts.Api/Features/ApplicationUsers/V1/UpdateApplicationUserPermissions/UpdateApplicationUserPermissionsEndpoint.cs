namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.UpdateApplicationUserPermissions;

public class UpdateApplicationUserPermissionsEndpoint(
    IApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
    : Endpoint<UpdateApplicationUserPermissionsRequest>
{
    public override void Configure()
    {
        Put("/api/v1/accounts/users/{UserId}/permissions");
        Description(x => x
            .WithName("UpdateUserPermissions")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
        );
    }

    public override async Task HandleAsync(UpdateApplicationUserPermissionsRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var loggedInUser = await applicationDbContext
            .ApplicationUsers
            .Include(a => a.Permissions)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
        
        var tenant = await applicationDbContext.Tenants.FindAsync([loggedInUser!.TenantId, ct], cancellationToken: ct);
        
        var user = await applicationDbContext
            .ApplicationUsers
            .Include(a => a.Permissions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == ApplicationUserId.Of(req.UserId), ct);

        if (user == null)
        {
            await SendErrorsAsync((int) HttpStatusCode.BadRequest, ct);
            return;
        }

        List<Permission> permissions = [];
        permissions.AddRange(req.Permissions.Select(permission => new Permission(Enum.Parse<PermissionType>(permission))));
        
        var result = user.UpdatePermission(tenant!, loggedInUser, permissions.ToArray(), req.OperationType);

        if (result.IsFailed)
        {
            await SendForbiddenAsync(ct);   
        }
        
        await applicationDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}

public class UpdateApplicationUserPermissionsRequestValidator : Validator<UpdateApplicationUserPermissionsRequest>
{
    public UpdateApplicationUserPermissionsRequestValidator()
    {
        RuleFor(x => x.Permissions)
            .NotEmpty()
            .Must(list => list.All(p => Enum.TryParse<PermissionType>(p, out _)))
            .WithMessage("All permissions must be valid PermissionType values.");
        
        RuleFor(x => x.OperationType)
            .NotEmpty()
            .Must(op => op is "Add" or "Remove")
                .WithMessage("OperationType must be either 'Add' or 'Remove'.");
        
    }
}