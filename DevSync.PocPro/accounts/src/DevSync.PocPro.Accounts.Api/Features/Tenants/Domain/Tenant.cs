namespace DevSync.PocPro.Accounts.Api.Features.Tenants.Domain;

public class Tenant : BaseEntity<TenantId>
{
    public static Tenant Create(string uniqueIdentifier, string connectionString, SubscriptionType subscriptionType)
    {
        var tenant = new Tenant()
        {
            Id = TenantId.Of(Guid.CreateVersion7()),
            UniqueIdentifier = uniqueIdentifier,
            ConnectionString = connectionString,
            SubscriptionType = subscriptionType
        };
        
        return tenant;
    }

    public string UniqueIdentifier { get; private set; }
    public string ConnectionString { get; private set; }
    public SubscriptionType? SubscriptionType { get; private set; }
    
    public Result UpdateSubscription(SubscriptionType subscriptionType, string userId)
    {
        if (userId != CreatedBy)
        {
            return Result.Fail("Only the creator can update the subscription.");
        }
        
        SubscriptionType = subscriptionType;
        
        return Result.Ok();
    }
}

public record TenantId
{
    public Guid Value { get; } = Guid.Empty;

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