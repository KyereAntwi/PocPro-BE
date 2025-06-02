namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CheckIfUserHasRequiredPermission;

public record CheckIfUserHasRequiredPermissionRequest([FromRoute]string UserId, string PermissionType);

public record CheckIfUserHasRequiredPermissionResponse(bool HasPermission);