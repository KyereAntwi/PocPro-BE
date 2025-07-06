namespace DevSync.PocPro.Shops.UserWishlistModule.Data;

public class UserWishListModuleDbContext : DbContext, IWishListDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantServices _tenantServices;

    public UserWishListModuleDbContext(
        DbContextOptions<UserWishListModuleDbContext> options, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices)
        : base(options)
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
        var tenantIdentifier = _httpContextAccessor.HttpContext!.Request.Headers["X-Tenant-Identifier"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(tenantIdentifier)) 
            throw new BadRequestException("Missing User id or identifier from the request headers.");

        Tenant? tenant = null;

        if (!string.IsNullOrWhiteSpace(tenantIdentifier))
        {
            tenant = _tenantServices.GetTenantByIdentifierAsync(tenantIdentifier).Result!;
            if (tenant == null) throw new BadRequestException("Invalid Tenant from the request headers.");
        }

        optionsBuilder.UseNpgsql(tenant!.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserWishListModuleDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<WishListItem> WishListItems => Set<WishListItem>();
}