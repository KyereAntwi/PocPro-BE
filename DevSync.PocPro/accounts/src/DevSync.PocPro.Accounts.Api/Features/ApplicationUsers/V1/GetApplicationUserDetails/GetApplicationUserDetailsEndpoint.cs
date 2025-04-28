namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.GetApplicationUserDetails;

public class GetApplicationUserDetailsEndpoint(IApplicationDbContext applicationDbContext) 
    : Endpoint<GetApplicationUserDetailsRequest, BaseResponse<GetApplicationUserDetailsResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/users/{UserId}");
        Description(x => x
            .WithName("GetUserDetails")
            .Produces<BaseResponse<GetApplicationUserDetailsResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
        );
    }

    public override async Task HandleAsync(GetApplicationUserDetailsRequest req, CancellationToken ct)
    {
        var user = await applicationDbContext
            .ApplicationUsers
            .Include(u => u.Permissions)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == req.UserId, ct);

        if (user == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(new BaseResponse<GetApplicationUserDetailsResponse>("User details retrieved successfully", true)
        {
            Data = new GetApplicationUserDetailsResponse(
                user.FirstName,
                user.LastName,
                user.Email ?? string.Empty,
                user.OtherNames ?? string.Empty,
                user.UserId,
                user.PhotoUrl ?? string.Empty,
                user.Permissions.Select(p => p.PermissionType.ToString()))
        }, cancellation: ct);
    }
}