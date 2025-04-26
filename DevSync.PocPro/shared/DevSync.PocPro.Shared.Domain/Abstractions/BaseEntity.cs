namespace DevSync.PocPro.Shared.Domain.Abstractions;

public abstract class BaseEntity<T> : IEntity<T>
{
    public DateTimeOffset? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public T Id { get; set; }
}