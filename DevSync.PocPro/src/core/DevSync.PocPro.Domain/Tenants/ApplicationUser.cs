namespace DevSync.PocPro.Domain.Tenants;

public class ApplicationUser : BaseEntity<ApplicationUserId>
{
    private ApplicationUser()
    {
    }

    internal ApplicationUser(TenantId tenantId, string userId)
    {
        TenantId = tenantId;
        UserId = userId;
    }

    public TenantId TenantId { get; set; }
    public string UserId { get; set; }
}