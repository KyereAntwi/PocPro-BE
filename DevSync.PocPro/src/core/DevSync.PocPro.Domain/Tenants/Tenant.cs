using System.Collections.ObjectModel;

namespace DevSync.PocPro.Domain.Tenants;

public class Tenant : BaseEntity<TenantId>
{
    private readonly Collection<ApplicationUser> _applicationUsers = [];
    public IReadOnlyCollection<ApplicationUser> ApplicationUsers => _applicationUsers;
    public required string ConnectionString { get; set; }
    public required string UniqueIdentifier { get; set; }

    public static Tenant Create(TenantId id, string connectionString, string uniqueIdentifier)
    {
        var tenant = new Tenant
        {
            Id = TenantId.Of(Guid.NewGuid()),
            ConnectionString = connectionString,
            UniqueIdentifier = uniqueIdentifier
        };

        return tenant;
    }
    
    public void AddApplicationUser(string userId)
    {
        var newApplicationUser = new ApplicationUser(Id, userId);
        _applicationUsers.Add(newApplicationUser);
    }
}