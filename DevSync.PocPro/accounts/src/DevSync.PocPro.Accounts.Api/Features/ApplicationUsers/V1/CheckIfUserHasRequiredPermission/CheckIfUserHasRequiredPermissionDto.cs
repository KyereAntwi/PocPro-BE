namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.CheckIfUserHasRequiredPermission;

public record CheckIfUserHasRequiredPermissionRequest([FromRoute]string UserId, [FromRoute]string PermissionType);

public record CheckIfUserHasRequiredPermissionResponse(bool HasPermission);