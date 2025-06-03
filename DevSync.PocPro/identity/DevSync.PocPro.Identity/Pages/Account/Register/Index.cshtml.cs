using DevSync.PocPro.Identity.Pages.Account.Login;
using Microsoft.AspNetCore.Identity;

namespace DevSync.PocPro.Identity.Pages.Account.Register;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;
    private readonly IEventService _events;
    private readonly UserManager<IdentityUser> _userManager;
    public ViewModel View { get; set; }
    [BindProperty] public InputModel Input { get; set; }

    public Index(
        SignInManager<IdentityUser> signInManager,
        IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IEventService events, 
        UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _interaction = interaction;
        _schemeProvider = schemeProvider;
        _identityProviderStore = identityProviderStore;
        _events = events;
        _userManager = userManager;
    }
    
    public IActionResult OnGet(string returnUrl)
    {
        if (View.IsExternalLoginOnly)
        {
            // we only have one option for logging in and it's an external provider
            return RedirectToPage("/ExternalLogin/Challenge", new { scheme = View.ExternalLoginScheme, returnUrl });
        }

        return Page();
    }

    public async Task<IActionResult> OnPost(string returnUrl)
    {
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByNameAsync(Input.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return Page();
            }
            
            var user = new IdentityUser
            {
                UserName = Input.Username,
                Email = Input.Username,
                EmailConfirmed = true
            };
            
            var result = await _userManager.CreateAsync(user, Input.Password);
            
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));
                
                if (context != null)
                {
                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(returnUrl);
                }
                
                // request a redirect after we sign in the user
                return Redirect("~/");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        
        // something went wrong, show form with error
        await BuildModelAsync(Input.ReturnUrl);
        return Page();
    }
    
    private async Task BuildModelAsync(string returnUrl)
    {
        Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

            // this is meant to short circuit the UI and only trigger the one external IdP
            View = new ViewModel
            {
                EnableLocalLogin = local,
            };

            Input.Username = context?.LoginHint!;

            if (!local)
            {
                View.ExternalProviders = new[]
                    { new ViewModel.ExternalProvider { AuthenticationScheme = context.IdP } };
            }

            return;
        }

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var dyanmicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName
            });
        providers.AddRange(dyanmicSchemes);


        var allowLocal = true;
        var client = context?.Client;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin;
            if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
            {
                providers = providers.Where(provider =>
                    client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
            }
        }

        View = new ViewModel
        {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = providers.ToArray()
        };
    }
}