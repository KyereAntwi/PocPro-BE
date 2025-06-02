namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetTenantDetails;

public record GetTenantDetailsRequest([FromRoute] string UserId);

public record GetTenantDetailsResponse(string ConnectionString, Guid TenantId, string UniqueIdentifier, string? SubscriptionType);