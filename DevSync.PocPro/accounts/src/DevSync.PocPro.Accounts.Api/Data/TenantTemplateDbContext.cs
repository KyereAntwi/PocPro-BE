namespace DevSync.PocPro.Accounts.Api.Data;

public class TenantTemplateDbContext : DbContext
{
    public TenantTemplateDbContext(DbContextOptions<TenantTemplateDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantTemplateDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}