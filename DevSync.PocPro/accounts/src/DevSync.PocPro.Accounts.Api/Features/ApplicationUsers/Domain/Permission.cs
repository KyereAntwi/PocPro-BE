namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.Domain;

public class Permission : BaseEntity<PermissionId>
{
    private Permission()
    {
    }
    
    public Permission(PermissionType type)
    {
        PermissionType = type;
    }

    public PermissionType PermissionType { get; set; }
}

public record PermissionId
{
    public Guid Value { get; set; } = Guid.Empty;

    private PermissionId(Guid value) => Value = value;

    public static PermissionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for PermissionId");
        }

        return new PermissionId(value);
    }
}