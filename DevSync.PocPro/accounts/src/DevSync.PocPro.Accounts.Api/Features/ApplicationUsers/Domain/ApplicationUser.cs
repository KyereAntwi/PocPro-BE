namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.Domain;

public class ApplicationUser : BaseEntity<ApplicationUserId>
{
    private readonly Collection<Permission> _permissions = [];
    public IReadOnlyCollection<Permission> Permissions => _permissions;
    
    public static ApplicationUser Create(string firstName, string lastName, string otherNames, string email, 
        string userId, string photoUrl, List<Permission> permissions, TenantId? tenantId = null)
    {
        var application = new ApplicationUser
        {
            TenantId = tenantId,
            Id = ApplicationUserId.Of(Guid.CreateVersion7()),
            FirstName = firstName,
            LastName = lastName,
            OtherNames = otherNames,
            Email = email,
            UserId = userId,
            PhotoUrl = photoUrl
        };

        if (permissions.Count <= 0) return application;
        
        foreach (var permission in permissions)
        {
            application._permissions.Add(permission);
        }

        return application;
    }

    public TenantId? TenantId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? OtherNames { get; private set; }
    public string? Email { get; private set; }
    public string UserId { get; private set; }
    public string? PhotoUrl { get; private set; }
    
    public Result UpdatePermission(Tenant tenant, ApplicationUser user, Permission[] permissions)
    {
        if (!user.Permissions.Select(p => p.PermissionType).Contains(PermissionType.MANAGE_PERMISSIONS))
        {
            return Result.Fail($"User {user.UserId} cannot does not have permission to manage other user's permissions.");
        }
        
        if (TenantId == null)
        {
            return Result.Fail("This user cannot be assigned extra permissions");
        }
        
        if (tenant.CreatedBy != user.UserId)
        {
            return Result.Fail(
                $"User with userId {user.UserId} is not permitted to assign permission to users in tenant {tenant}");
        }
        
        _permissions.Clear();
        
        foreach (var permission in permissions)
        {
            // if (_permissions.Contains(permission))
            // {
            //     return Result.Fail($"User already has this permission. {permission.PermissionType}");
            // }

            _permissions.Add(permission);
        }

        // switch (type)
        // {
        //     case "Add":
        //     {
        //         foreach (var permission in permissions)
        //         {
        //             if (_permissions.Contains(permission))
        //             {
        //                 return Result.Fail($"User already has this permission. {permission.PermissionType}");
        //             }
        //
        //             _permissions.Add(permission);
        //         }
        //
        //         break;
        //     }
        //     case "Remove":
        //     {
        //         foreach (var permission in permissions)
        //         {
        //             if (!_permissions.Contains(permission))
        //             {
        //                 return Result.Fail($"User does not have this permission. {permission.PermissionType}");
        //             }
        //
        //             _permissions.Remove(permission);
        //         }
        //
        //         break;
        //     }
        //     default:
        //         return Result.Fail("Invalid operation type");
        // }
        
        return Result.Ok();
    }

    public bool HasPermission(PermissionType permissionType)
    {
        return _permissions.Select(p => p.PermissionType).Contains(permissionType);
    }
    
    public void Update(string firstName, string lastName, string? otherNames, string? email, string? photoUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        OtherNames = otherNames;
        Email = email;
        PhotoUrl = photoUrl;
    }
}

public record ApplicationUserId
{
    public Guid Value { get; } = Guid.Empty!;

    private ApplicationUserId(Guid value) => Value = value;

    public static ApplicationUserId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("User Id cannot be empty or null");
        }

        return new ApplicationUserId(value);
    }
}