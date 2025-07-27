namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.CreateApplicationUser;

public record CreateApplicationUserRequest(
    string Email,
    string UserId,
    string FirstName,
    string LastName,
    string? PhotoUrl);