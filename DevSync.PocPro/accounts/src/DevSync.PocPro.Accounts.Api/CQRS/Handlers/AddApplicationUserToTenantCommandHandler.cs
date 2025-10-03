using DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.AddApplicationUser;

namespace DevSync.PocPro.Accounts.Api.CQRS.Handlers;

public class AddApplicationUserToTenantCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IApplicationDbContext applicationDbContext,
    IIdentityServices identityServices) 
    : Shared.Domain.CQRS.ICommandHandler<AddApplicationUserRequest, AddApplicationUserResponse>
{
    public async Task<Result<AddApplicationUserResponse>> HandleAsync(AddApplicationUserRequest command, CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var newUserId = string.Empty;

        if (!command.TenantAccount)
        {
            var loggedInUser = await applicationDbContext
                .ApplicationUsers
                .Include(a => a.Permissions)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
            if (loggedInUser == null)
            {
                return Result.Fail("Logged in user not found");
            }

            if (!loggedInUser.HasPermission(PermissionType.MANAGE_USERS))
            {
                return Result.Fail("User does not have permission to manage users");
            }
        }

        try
        {
            newUserId = await identityServices.RegisterUserLoginAsync(command.Username, command.Email!, command.Password!,
                 httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault()!);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error registering user for identity. Error: {ex.Message}");
        }

        var existingUser = await applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(a => a.UserId == command.Username, cancellationToken);

        if (existingUser != null)
        {
            return Result.Fail("User with the same username already exists");
        }

        List<Permission> permissions = [];

        var photoUrl = string.Empty;
        if (command.PhotoFile != null)
        {
            //TODO - to be implemented
        }

        if (command.PermissionTypes != null && command.PermissionTypes.Any())
        {
            var permissionTypeEnums = command.PermissionTypes
                .Select(Enum.Parse<PermissionType>)
                .ToList();

            permissions = await applicationDbContext.Permissions
                .Where(p => permissionTypeEnums.Contains(p.PermissionType))
                .ToListAsync(cancellationToken);
        }

        var result = ApplicationUser.Create(command.FirstName, command.LastName, command.OtherNames ?? string.Empty, command.Email ?? string.Empty, newUserId, photoUrl,
            permissions, TenantId.Of(command.TenantId));

        await applicationDbContext.ApplicationUsers.AddAsync(result, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(new AddApplicationUserResponse(result.Id.Value));
    }
}
