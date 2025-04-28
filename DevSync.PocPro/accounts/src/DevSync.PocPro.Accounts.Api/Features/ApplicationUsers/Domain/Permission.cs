namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.Domain;

public class Permission : BaseEntity<PermissionId>
{
    private Permission()
    {
    }
    
    public Permission(PermissionType type)
    {
        Id = PermissionId.Of(Guid.NewGuid());
        PermissionType = type;
    }

    public PermissionType PermissionType { get; set; }
}

public record PermissionId
{
    public Guid Value { get; } = Guid.Empty!;

    private PermissionId(Guid value) => Value = value;

    public static PermissionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Permission Id cannot be empty or null");
        }

        return new PermissionId(value);
    }
}