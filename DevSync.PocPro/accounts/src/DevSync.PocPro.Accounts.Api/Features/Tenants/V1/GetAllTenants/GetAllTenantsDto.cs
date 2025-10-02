namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetAllTenants;

public record GetAllTenantsResponse(IEnumerable<GetTenantDetailsResponse> Tenants);