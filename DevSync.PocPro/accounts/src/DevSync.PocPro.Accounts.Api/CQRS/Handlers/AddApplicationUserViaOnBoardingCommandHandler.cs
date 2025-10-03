using DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.AddApplicationUserViaOnBoarding;

namespace DevSync.PocPro.Accounts.Api.CQRS.Handlers;

public class AddApplicationUserViaOnBoardingCommandHandler(
    IApplicationDbContext applicationDbContext,
    IIdentityServices identityServices) 
    : Shared.Domain.CQRS.ICommandHandler<AddApplicationUserViaOnBoardingRequest, Guid>
{
    public async Task<Result<Guid>> HandleAsync(AddApplicationUserViaOnBoardingRequest command, CancellationToken cancellationToken = default)
    {
        var newUserId = string.Empty;

        try
        {
            newUserId = await identityServices.RegisterUserLoginAsync(command.Username, command.Email!, command.Password!, null);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error registering user for identity. Error: {ex.Message}");
        }

        var existingUser = await applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(a => a.Email == command.Email, cancellationToken);

        if (existingUser != null)
        {
            return Result.Fail("User with the same username already exists");
        }

        List<Permission> permissions = [];

        if (command.PermissionTypes != null && command.PermissionTypes.Any())
        {
            var permissionTypeEnums = command.PermissionTypes
                .Select(Enum.Parse<PermissionType>)
                .ToList();

            permissions = await applicationDbContext.Permissions
                .Where(p => permissionTypeEnums.Contains(p.PermissionType))
                .ToListAsync(cancellationToken);
        }

        var existingTenant = await applicationDbContext.Tenants.FirstOrDefaultAsync(t => t.UniqueIdentifier == command.TenantIdentifier, cancellationToken);

        if (existingTenant == null) 
        { 
            return Result.Fail("Tenant not found");
        }

        var result = ApplicationUser.Create(command.FirstName, command.LastName, command.OtherNames ?? string.Empty, command.Email ?? string.Empty, newUserId, string.Empty, permissions, existingTenant.Id);

        await applicationDbContext.ApplicationUsers.AddAsync(result, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(result.Id.Value);
    }
}
