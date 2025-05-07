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
        
        var loggedInUser = await applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.UserId == userId, ct);
        var tenant = await applicationDbContext.Tenants.FindAsync([loggedInUser!.TenantId, ct], cancellationToken: ct);
        var user = await applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.UserId == req.UserId, ct);

        if (user == null)
        {
            await SendErrorsAsync((int) HttpStatusCode.BadRequest, ct);
            return;
        }

        List<Permission> permissions = [];

        var permissionTasks = req.Permissions.Select(async permission =>
        {
            var existingPermission = await applicationDbContext.Permissions.FirstOrDefaultAsync(
                x => x.PermissionType == Enum.Parse<PermissionType>(permission), ct);

            if (existingPermission != null) return existingPermission;
            await SendNotFoundAsync(ct);
            return null;
        });
        
        var resolvedPermissions = await Task.WhenAll(permissionTasks);
        
        if (resolvedPermissions.Any(p => p == null))
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        permissions.AddRange(resolvedPermissions.Where(p => p != null)!);
        
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
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Permissions).NotEmpty();
        RuleFor(x => x.OperationType).NotEmpty();
    }
}