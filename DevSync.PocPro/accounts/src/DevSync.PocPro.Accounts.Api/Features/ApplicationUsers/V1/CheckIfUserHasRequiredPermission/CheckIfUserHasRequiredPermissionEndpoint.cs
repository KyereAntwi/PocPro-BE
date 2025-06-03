namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.CheckIfUserHasRequiredPermission;

public class CheckIfUserHasRequiredPermissionEndpoint(IApplicationDbContext applicationDbContext) 
    : Endpoint<CheckIfUserHasRequiredPermissionRequest, BaseResponse<CheckIfUserHasRequiredPermissionResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/users/{UserId}/permissions/{PermissionType}");
    }

    public override async Task HandleAsync(CheckIfUserHasRequiredPermissionRequest req, CancellationToken ct)
    {
        var user = await applicationDbContext
            .ApplicationUsers
            .Include(a => a.Permissions)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.UserId == req.UserId, ct);

        if (user == null)
        {
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
        }

        await SendOkAsync(new BaseResponse<CheckIfUserHasRequiredPermissionResponse>("Success", true)
        {
            Data = !user!.HasPermission(Enum.Parse<PermissionType>(req.PermissionType)) 
                ? new CheckIfUserHasRequiredPermissionResponse(false) 
                : new CheckIfUserHasRequiredPermissionResponse(true)
        }, ct);
    }
}