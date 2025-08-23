namespace DevSync.PocPro.Accounts.Api.Features.Tenants.Domain;

public class SubAccount : BaseEntity<SubAccountId>
{
    private SubAccount()
    {
    }
    
    public SubAccount(TenantId tenantId, string businessName, string settlementBank, string accountNumber, float percentageCharge)
    {
        Id = SubAccountId.Of(Guid.NewGuid());
        TenantId = tenantId;
        BusinessName = businessName;
        SettlementBank = settlementBank;
        AccountNumber = accountNumber;
        PercentageCharge = percentageCharge;
    }
    
    public TenantId TenantId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string SettlementBank { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public float PercentageCharge { get; set; } = 100.0f;
}

public record SubAccountId
{
    public Guid Value { get; } = Guid.Empty;

    private SubAccountId(Guid value) => Value = value;

    public static SubAccountId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for SubAccountId");
        }
        
        return new SubAccountId(value);
    }
}