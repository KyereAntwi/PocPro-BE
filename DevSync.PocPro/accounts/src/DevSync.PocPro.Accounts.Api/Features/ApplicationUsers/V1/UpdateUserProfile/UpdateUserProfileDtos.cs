namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.UpdateUserProfile;

public record UpdateUserProfileRequest(
    [FromRoute] string UserId,
    string FirstName,
    string LastName,
    string OtherNames,
    string Email,
    IFormFile PhotoUrl);