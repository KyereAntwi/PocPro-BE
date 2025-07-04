// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using DevSync.PocPro.Identity.Pages;
using Duende.IdentityServer.Models;

namespace Devsync.Identity.Pages.Ciba;

[AllowAnonymous]
[SecurityHeaders]
public class IndexModel : PageModel
{
    public BackchannelUserLoginRequest LoginRequest { get; set; }

    private readonly IBackchannelAuthenticationInteractionService _backchannelAuthenticationInteraction;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService,
        ILogger<IndexModel> logger)
    {
        _backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet(string id)
    {
        LoginRequest = await _backchannelAuthenticationInteraction.GetLoginRequestByInternalIdAsync(id);
        if (LoginRequest == null)
        {
            _logger.LogWarning("Invalid backchannel login id {id}", id);
            return RedirectToPage("/Home/Error/Index");
        }

        return Page();
    }
}