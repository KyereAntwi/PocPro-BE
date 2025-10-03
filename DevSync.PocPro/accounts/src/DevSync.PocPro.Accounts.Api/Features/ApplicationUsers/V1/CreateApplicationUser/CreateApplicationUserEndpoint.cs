namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.CreateApplicationUser;

public class CreateApplicationUserEndpoint(
    IApplicationDbContext applicationDbContext) : Endpoint<CreateApplicationUserRequest>
{
    public override void Configure()
    {
        Post("/api/v1/accounts");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateApplicationUserRequest req, CancellationToken ct)
    {
        var existingUser = await applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == req.Email, ct);

        if (existingUser is not null)
        {
            await SendErrorsAsync((int)HttpStatusCode.BadRequest, ct);
            return;
        }

        var permission =
            await applicationDbContext
                .Permissions
                .FirstAsync(p => p.PermissionType == PermissionType.VIEW_ORDERS, ct);

        var newUser = ApplicationUser.Create(
            req.FirstName,
            req.LastName,
            string.Empty,
            req.Email,
            req.UserId,
            req.PhotoUrl ?? string.Empty,
            [permission],
            null);
        
        await applicationDbContext.ApplicationUsers.AddAsync(newUser, ct);
        await applicationDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}