namespace DevSync.PocPro.WorkerService.Utils;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if (!httpContext.User.Identity?.IsAuthenticated ?? false)
        {
            return false;
        }
        
        return httpContext.User.IsInRole("SystemAdmin");
    }
}