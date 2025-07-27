using System.Security.Claims;
using DevSync.PocPro.Identity.Dtos;
using DevSync.PocPro.Identity.Services;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Identity;

namespace DevSync.PocPro.Identity.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAccountsApiServices _accountsApiServices;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<Callback> _logger;
    private readonly IEventService _events;

    public Callback(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<Callback> logger,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IAccountsApiServices accountsApiServices)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _accountsApiServices = accountsApiServices;
        _interaction = interaction;
        _logger = logger;
        _events = events;
    }

    public async Task<IActionResult> OnGet()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result.Succeeded != true)
        {
            throw new Exception("External authentication error");
        }

        var externalUser = result.Principal;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = externalUser!.Claims.Select(c => $"{c.Type}: {c.Value}");
            _logger.LogDebug("External claims: {@claims}", externalClaims);
        }
        
        var userIdClaim = externalUser!.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new Exception("Unknown userid");

        var provider = result.Properties!.Items["scheme"];
        var providerUserId = userIdClaim.Value;
        
        var user = await _userManager.FindByLoginAsync(provider!, providerUserId);
        if (user == null)
        {
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);
            
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
            var profilePicture = claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            
            user = new IdentityUser
            {
                Id = providerUserId,
                UserName = email ?? providerUserId,
                Email = email,
                NormalizedUserName = email!.ToUpper(),
                NormalizedEmail = email.ToUpper()
            };
            
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user, new UserLoginInfo(provider!, providerUserId, provider));
            await _userManager.AddClaimsAsync(user, claims);
            
            // TODO - send request to account user api
            await _accountsApiServices.AddUserToAccountsAsync(new AddUserToAccountRequest(
                email!,
                providerUserId,
                firstName!,
                lastName!,
                profilePicture ?? string.Empty));
        }
        
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

        await _signInManager.SignInAsync(user, isPersistent: false);

        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        
        var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";
        
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, user.UserName,
            true, context?.Client.ClientId));

        if (context == null) return Redirect(returnUrl);
        
        return context.IsNativeClient() ? this.LoadingPage(returnUrl) : Redirect(returnUrl);
    }
    
    private void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims,
        AuthenticationProperties localSignInProps)
    {
        var sid = externalResult.Principal!.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        var idToken = externalResult.Properties!.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
        }
    }
}