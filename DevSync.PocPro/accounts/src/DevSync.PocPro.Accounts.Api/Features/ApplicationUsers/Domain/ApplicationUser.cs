namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.Domain;

public class ApplicationUser : BaseEntity<ApplicationUserId>
{
    private readonly Collection<Permission> _permissions = [];
    public IReadOnlyCollection<Permission> Permissions => _permissions;
    
    public static ApplicationUser Create(
        Guid tenantId, string firstName, string lastName, string otherNames, string email, string userId, string photoUrl, Permission[] permissions)
    {
        var application = new ApplicationUser
        {
            Id = ApplicationUserId.Of(Guid.CreateVersion7()),
            TenantId = TenantId.Of(tenantId),
            FirstName = firstName,
            LastName = lastName,
            OtherNames = otherNames,
            Email = email,
            UserId = userId,
            PhotoUrl = photoUrl
        };

        if (permissions.Length <= 0) return application;
        
        foreach (var permission in permissions)
        {
            application._permissions.Add(permission);
        }

        return application;
    }

    public TenantId? TenantId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? OtherNames { get; set; }
    public string? Email { get; set; }
    public required string UserId { get; set; }
    public string? PhotoUrl { get; set; }
    
    public Result AddPermission(Tenant tenant, ApplicationUser user, Permission permission)
    {
        if (!user.Permissions.Select(p => p.PermissionType).Contains(PermissionType.MANAGE_PERMISSIONS))
        {
            return Result.Fail($"User {user.UserId} cannot does not have permission to manage other user's permissions.");
        }
        
        if (this.TenantId == null)
        {
            return Result.Fail("This user cannot be assigned extra permissions");
        }
        
        if (tenant.CreatedBy != user.UserId)
        {
            return Result.Fail(
                $"User with userId {user.UserId} is not permitted to assign permission to users in tenant {tenant}");
        }
        
        if (_permissions.Contains(permission))
        {
            return Result.Fail($"User already has this permission. {permission.PermissionType}");
        }

        _permissions.Add(permission);
        
        return Result.Ok();
    }
    
    public Result RemovePermission(Permission permission)
    {
        if (!_permissions.Contains(permission))
        {
            return Result.Fail($"User does not have this permission. {permission.PermissionType}");
        }

        _permissions.Remove(permission);
        
        return Result.Ok();
    }
}

public record ApplicationUserId
{
    public Guid Value { get; set; } = Guid.Empty;

    private ApplicationUserId(Guid value) => Value = value;

    public static ApplicationUserId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for ApplicationUserId");
        }

        return new ApplicationUserId(value);
    }
}