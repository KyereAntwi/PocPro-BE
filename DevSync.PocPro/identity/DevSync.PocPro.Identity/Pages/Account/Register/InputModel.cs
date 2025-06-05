namespace DevSync.PocPro.Identity.Pages.Account.Register;

public class InputModel
{
    [Required] 
    [EmailAddress]
    public string Username { get; set; }

    [Required] 
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; }
    
    [Required] 
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    public string ReturnUrl { get; set; }
    
    public bool RememberLogin { get; set; }

    public string Button { get; set; }
}