namespace DevSync.PocPro.Shops.Api.Data;

public class MainShopTemplateDbContext : DbContext
{
    public MainShopTemplateDbContext(DbContextOptions<MainShopTemplateDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StocksModuleDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(POSModuleDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersModuleDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerModuleDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<PointOfSale> PointOfSales => Set<PointOfSale>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ShippingAddress> ShippingAddresses => Set<ShippingAddress>();
    public DbSet<Customer> Customers => Set<Customer>();
}