namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.GetApplicationUserDetails;

public record GetApplicationUserDetailsResponse(
    string FirstName,
    string LastName,
    string Email,
    string OtherNames,
    string UserId,
    string PhotoUrl,
    IEnumerable<string> Permissions);
    
public record GetApplicationUserDetailsRequest(string UserId);