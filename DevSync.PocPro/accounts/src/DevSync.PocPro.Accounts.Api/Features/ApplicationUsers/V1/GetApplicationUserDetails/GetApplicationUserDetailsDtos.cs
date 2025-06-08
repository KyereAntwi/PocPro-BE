namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.GetApplicationUserDetails;

public record GetApplicationUserDetailsResponse(
    Guid Id,
    Guid TenantId,
    string FirstName,
    string LastName,
    string Email,
    string OtherNames,
    string Username,
    string PhotoUrl,
    IEnumerable<string> Permissions);
    
public record GetApplicationUserDetailsRequest(string UserId);