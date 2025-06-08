namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.AddApplicationUser;

public record AddApplicationUserRequest(
    string FirstName,
    string LastName,
    string? Email,
    string Username,
    string? OtherNames,
    string? Password,
    IFormFile? PhotoFile,
    IEnumerable<string>? PermissionTypes,
    Guid TenantId,
    bool TenantAccount);

public record AddApplicationUserResponse(Guid Id);