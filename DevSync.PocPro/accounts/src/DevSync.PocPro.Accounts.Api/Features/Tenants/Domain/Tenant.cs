namespace DevSync.PocPro.Accounts.Api.Features.Tenants.Domain;

public class Tenant : BaseEntity<TenantId>
{
    
}

public record TenantId
{
    public Guid Value { get; set; } = Guid.Empty;

    private TenantId(Guid value) => Value = value;

    public static TenantId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for TenantId");
        }

        return new TenantId(value);
    }
}