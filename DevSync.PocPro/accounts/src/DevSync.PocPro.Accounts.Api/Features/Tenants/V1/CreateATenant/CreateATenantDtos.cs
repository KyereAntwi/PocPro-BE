namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateATenant;

public record CreateATenantRequest(string UniqueIdentifier, string SubscriptionType);

public record CreateATenantResponse(Guid Id);