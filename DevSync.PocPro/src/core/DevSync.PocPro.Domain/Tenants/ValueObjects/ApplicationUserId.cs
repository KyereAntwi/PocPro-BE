namespace DevSync.PocPro.Domain.Tenants.ValueObjects;

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