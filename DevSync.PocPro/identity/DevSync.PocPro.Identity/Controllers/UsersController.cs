using DevSync.PocPro.Identity.Dtos;
using Microsoft.AspNetCore.Identity;

namespace DevSync.PocPro.Identity.Controllers;

[Route("/api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var existingUser = await _userManager.FindByNameAsync(request.Username);

        if (existingUser != null)
        {
            return BadRequest("User already exists.");
        }

        var user = new IdentityUser
        {
            UserName = request.Username,
            Email = request.Email ?? string.Empty
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            // TODO - Return back for reverse
        }
        
        var currentUser = await _userManager.FindByNameAsync(request.Username);
        return Ok(currentUser!.Id);
    }
}