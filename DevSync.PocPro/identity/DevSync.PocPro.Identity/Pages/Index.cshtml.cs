using System.Reflection;

namespace DevSync.PocPro.Identity.Pages;

[AllowAnonymous]
public class Index : PageModel
{
    public string Version;

    public void OnGet()
    {
        Version = typeof(Duende.IdentityServer.Hosting.IdentityServerMiddleware).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').First();
    }
}