namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.AddApplicationUser;

public record AddApplicationUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string? OtherNames,
    IFormFile? PhotoFile,
    IEnumerable<string>? PermissionTypes,
    Guid TenantId);

public record AddApplicationUserResponse(Guid Id);