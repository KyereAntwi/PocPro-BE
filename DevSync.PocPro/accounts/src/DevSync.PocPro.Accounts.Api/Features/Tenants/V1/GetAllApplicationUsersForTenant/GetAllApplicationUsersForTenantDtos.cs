namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetAllApplicationUsersForTenant;

public record GetAllApplicationUsersForTenantRequest(Guid Id, string SearchKeyword);

public record GetApplicationUsersForTenantResponse(
    string UserId,
    string FirstName,
    string LastName,
    string Email,
    string OtherName,
    string PhotoUrl);