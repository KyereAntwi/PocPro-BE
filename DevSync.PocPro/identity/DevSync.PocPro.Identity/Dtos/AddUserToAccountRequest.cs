namespace DevSync.PocPro.Identity.Dtos;

public record AddUserToAccountRequest(
    string Email,
    string UserId,
    string FirstName,
    string LastName,
    string? PhotoUrl);