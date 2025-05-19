using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DevSync.PocPro.Identity.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}