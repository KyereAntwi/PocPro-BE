namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.UpdateApplicationUserPermissions;

public record UpdateApplicationUserPermissionsRequest(
    [FromRoute] string UserId,
    List<string> Permissions,
    [Microsoft.AspNetCore.Mvc.FromQuery] string OperationType);