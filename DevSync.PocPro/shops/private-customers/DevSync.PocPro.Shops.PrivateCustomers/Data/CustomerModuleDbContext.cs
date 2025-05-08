namespace DevSync.PocPro.Shops.PrivateCustomers.Data;

public class CustomerModuleDbContext : DbContext, ICustomerDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantServices _tenantServices;
    
    public CustomerModuleDbContext(DbContextOptions<CustomerModuleDbContext> options, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantServices = tenantServices;
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entity in ChangeTracker.Entries<IEntity>())
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Entity.CreatedBy = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    entity.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entity.Entity.UpdatedAt = DateTime.UtcNow;
                    entity.Entity.UpdatedBy = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    break;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var userId = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId)) throw new BadRequestException("Missing User id from the request headers.");
        
        var tenant = _tenantServices.GetTenantByUserIdAsync(userId).Result;

        if (tenant == null) throw new BadRequestException("Invalid Tenant from the request headers.");
            
        optionsBuilder.UseNpgsql(tenant.ConnectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerModuleDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Customer> Customers => Set<Customer>();
}