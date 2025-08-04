using DevSync.PocPro.Shared.Domain.Middlewares;
using DevSync.PocPro.Shops.GeneralSettings.DI;
using DevSync.Pocpro.Shops.Notifications.DI;
using DevSync.PocPro.Shops.OrdersModule.DI;
using DevSync.PocPro.Shops.PointOfSales.DI;
using DevSync.PocPro.Shops.PrivateCustomers.DI;
using DevSync.PocPro.Shops.ProductsQueryModule.DI;
using DevSync.PocPro.Shops.ReportsModule.DI;
using DevSync.PocPro.Shops.Reviews.DI;
using DevSync.PocPro.Shops.Shared.Utils;
using DevSync.PocPro.Shops.StocksModule.Services;
using DevSync.PocPro.Shops.UserWishlistModule.DI;

namespace DevSync.PocPro.Shops.Api.DI;

public static class Startup
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        var tenantDatabaseSettings = new TenantDatabaseSettings();
        builder.Configuration.GetSection("TenantDatabaseSettings").Bind(tenantDatabaseSettings);
        builder.Services.AddSingleton(tenantDatabaseSettings);

        var tenantServiceSettings = new TenantServiceSettings();
        builder.Configuration.GetSection("TenantServiceSettings").Bind(tenantServiceSettings);
        builder.Services.AddSingleton(tenantServiceSettings);

        var apiAuthentication = new ApiAuthentication();
        builder.Configuration.GetSection("ApiAuthentication").Bind(apiAuthentication);
        builder.Services.AddSingleton(apiAuthentication);
        
        var messageBroker = new MessageBroker();
        builder.Configuration.GetSection("MessageBroker").Bind(messageBroker);
        builder.Services.AddSingleton(messageBroker);
        
        builder.Services.AddStockModule(builder.Configuration);
        builder.Services.AddNotificationsModule(builder.Configuration);
        builder.Services.RegisterOrderModule();
        builder.Services.AddPosDependencies();
        builder.Services.AddCustomerModule();
        builder.Services.AddWishListModule();
        builder.Services.AddReportDependencies();
        builder.Services.AddGeneralSettingsModule();
        builder.Services.AddProductQueryDependencies(builder.Configuration);
        builder.Services.AddReviewsModuleDependencies();

        builder.Services.AddScoped<IMasterExtensions, MasterExtensions>();
        builder.Services.AddScoped<IPurchaseServices, PurchaseServices>();
        
        builder.Services.AddDbContext<MainShopTemplateDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        
        builder.Services.AddSingleton<ITenantServices, TenantServices>();
        builder.Services.AddTransient<ITenantRegistrationServices, TenantRegistrationServices>();

        builder.Services.AddSingleton<RabbitMqConnectionFactory>();
        builder.Services.AddSingleton<EventPublisher>();
        
        builder.Services.AddHostedService<GenerateTenantDatabaseEventHandler>();
        builder.Services.AddHostedService<AddProductToQueryEventHandler>();
        builder.Services.AddHostedService<PurchaseMadeOnProductEventHandler>();
        builder.Services.AddHostedService<SendProductRatedEventHandler>();
        builder.Services.AddHostedService<ProductUpdateEventHandler>();
        
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();

        // builder.Services.AddGrpcClient<TenantService.TenantServiceClient>(options =>
        // {
        //     options.Address = new Uri("https://devsyncaccountsapi-ccguashuc8a5gpfs.uksouth-01.azurewebsites.net");
        // }).AddPolicyHandler(_ => Policy<HttpResponseMessage>
        //         .Handle<Grpc.Core.RpcException>()
        //         .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
        
        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = builder.Configuration["Identity:Authority"];
                options.Audience = builder.Configuration["Identity:Audience"];
                options.TokenValidationParameters.ValidateAudience = true;
            });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "shop.api");
            });
        });
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Open", b =>
                b.SetIsOriginAllowed(origin => 
                        new Uri(origin).Host == "https://devsyncshopgateway-acdubue3e2eahkfs.uksouth-01.azurewebsites.net" ||
                        new Uri(origin).Host == "localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });
        
        builder.Services.AddFastEndpoints();
        
        return builder.Build();
    }
    
    public static WebApplication AddPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            //_ = app.ConfigureDatabaseAsync();
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("DevSync's PocPro Main Shops API")
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }
        
        app.UseCustomExceptionHandler();
        app.UseCors("Open");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHttpsRedirection();
        app.UseFastEndpoints();
        //app.MapHub<NotificationHub>("/hubs/notifications");
        
        return app;
    }
}