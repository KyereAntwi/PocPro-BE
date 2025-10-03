namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.AddApplicationUserViaOnBoarding;

public record AddApplicationUserViaOnBoardingRequest(
    string TenantIdentifier,
    string FirstName,
    string LastName,
    string? Email,
    string Username,
    string? OtherNames,
    string? Password,
    IEnumerable<string>? PermissionTypes);
