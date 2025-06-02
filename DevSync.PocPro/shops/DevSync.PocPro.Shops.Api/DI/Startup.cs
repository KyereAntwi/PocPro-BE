using DevSync.PocPro.Shared.Domain.Utils;
using DevSync.PocPro.Shops.Api.Services;
using DevSync.PocPro.Shops.OrdersModule.DI;
using DevSync.PocPro.Shops.PointOfSales.DI;
using DevSync.PocPro.Shops.PrivateCustomers.DI;

namespace DevSync.PocPro.Shops.Api.DI;

public static class Startup
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        var tenantDatabaseSettings = new TenantDatabaseSettings();
        builder.Configuration.GetSection("TenantDatabaseSettings").Bind(tenantDatabaseSettings);
        builder.Services.AddSingleton(tenantDatabaseSettings);
        
        builder.Services.AddStockModule(builder.Configuration);
        builder.Services.RegisterOrderModule();
        builder.Services.AddPosDependencies();
        builder.Services.AddCustomerModule();
        
        builder.Services.AddDbContext<MainShopTemplateDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        
        builder.Services.AddTransient<ITenantServices, TenantServices>();
        builder.Services.AddScoped<ITenantRegistrationServices, TenantRegistrationServices>();
        
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumer<GenerateTenantDatabaseEventHandler>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), host =>
                { 
                    host.Username(builder.Configuration["MessageBroker:Username"]!); 
                    host.Password(builder.Configuration["MessageBroker:Password"]!); 
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
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
                        new Uri(origin).Host == "http://localhost" ||
                        new Uri(origin).Host == "https://devsyncshopgateway-acdubue3e2eahkfs.uksouth-01.azurewebsites.net"
                        )
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
        
        app.UseCors("Open");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseFastEndpoints();
        app.UseHttpsRedirection();
        
        return app;
    }
}