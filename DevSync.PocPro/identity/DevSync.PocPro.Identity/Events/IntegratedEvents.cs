namespace DevSync.PocPro.Identity.Events;

public record IntegratedEvents
{
    public Guid Id => Guid.CreateVersion7();
    public DateTimeOffset OccuredOn => DateTimeOffset.Now;
    public string EventType => GetType().AssemblyQualifiedName!;
}