using Microsoft.EntityFrameworkCore;

namespace DevSync.PocPro.Accounts.Api.Data;

public class AccountsDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountsDbContext(
        DbContextOptions<AccountsDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // foreach (var entity in ChangeTracker.Entries<IEntity>())
        // {
        //     switch (entity.State)
        //     {
        //         case EntityState.Added:
        //             entity.Entity.CreatedBy = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        //             entity.Entity.CreatedAt = DateTime.UtcNow;
        //             break;
        //         case EntityState.Modified:
        //             entity.Entity.UpdatedAt = DateTime.UtcNow;
        //             entity.Entity.UpdatedBy = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        //             break;
        //     }
        // }
        
        return base.SaveChangesAsync(cancellationToken);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}