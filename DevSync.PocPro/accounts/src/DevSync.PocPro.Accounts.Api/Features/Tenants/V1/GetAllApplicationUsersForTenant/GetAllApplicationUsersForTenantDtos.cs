namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetAllApplicationUsersForTenant;

public record GetAllApplicationUsersForTenantRequest(Guid Id);

public record GetApplicationUsersForTenantResponse(
    string FirstName,
    string LastName,
    string Email,
    string OtherName,
    string PhotoUrl);