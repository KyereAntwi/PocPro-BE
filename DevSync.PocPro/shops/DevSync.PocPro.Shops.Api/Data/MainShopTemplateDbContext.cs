using DevSync.PocPro.Shops.PromoCodesModule.Data;
using DevSync.PocPro.Shops.PromoCodesModule.Domain;
using DevSync.PocPro.Shops.Reviews.Data;
using DevSync.PocPro.Shops.Reviews.Domains;

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserWishListModuleDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeneralSettingsModuleDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductReviewsModuleDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PromoCodeModuleDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<PointOfSale> PointOfSales => Set<PointOfSale>();
    public DbSet<PointOfSaleManager> PointOfSaleManagers => Set<PointOfSaleManager>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ShippingAddress> ShippingAddresses => Set<ShippingAddress>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ProductMedia> ProductMedias => Set<ProductMedia>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<WishListItem> WishListItems => Set<WishListItem>();
    public DbSet<Settings> Settings => Set<Settings>();
    public DbSet<SeoInformation> SeoInformation => Set<SeoInformation>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<PromoCodeUser> PromoCodeUsers => Set<PromoCodeUser>();
}