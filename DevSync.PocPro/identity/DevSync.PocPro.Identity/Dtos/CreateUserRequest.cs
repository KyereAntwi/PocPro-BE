namespace DevSync.PocPro.Identity.Dtos;

public record CreateUserRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [EmailAddress(ErrorMessage = "Should be a valid email")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")] 
    public string Password { get; set; } = string.Empty;
}