namespace DevSync.PocPro.Shared.Domain.Events;

public record DeleteCustomerEvent : IntegrationEvents
{
    public Guid CustomerId { get; set; }
}