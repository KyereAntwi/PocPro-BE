namespace DevSync.PocPro.Shared.Domain.ValueObjects;

public record CustomerId
{
    public Guid Value { get; } = Guid.Empty!;

    private CustomerId(Guid value) => Value = value;

    public static CustomerId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Customer Id cannot be empty or null");
        }

        return new CustomerId(value);
    }
}