namespace DevSync.PocPro.Shared.Domain.Events;

public record IntegrationEvents
{
    public Guid Id => Guid.CreateVersion7();
    public DateTimeOffset OccuredOn => DateTimeOffset.Now;
    public string EventType => GetType().AssemblyQualifiedName!;
}