namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.UpdateApplicationUserPermissions;

public record UpdateApplicationUserPermissionsRequest(
    [FromRoute] Guid UserId,
    List<string> Permissions,
    string OperationType);