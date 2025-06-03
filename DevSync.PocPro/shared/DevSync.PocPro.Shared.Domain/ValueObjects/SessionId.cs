namespace DevSync.PocPro.Shared.Domain.ValueObjects;

public record SessionId
{
    public Guid Value { get; } = Guid.Empty;
    
    private SessionId(Guid value) => Value = value;

    public static SessionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for SessionId");
        }
        
        return new SessionId(value);
    }
}