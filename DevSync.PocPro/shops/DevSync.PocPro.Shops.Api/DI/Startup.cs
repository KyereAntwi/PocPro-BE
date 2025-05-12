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

        builder.Services.AddGrpcClient<TenantService.TenantServiceClient>(options =>
        {
            options.Address = new Uri("https://localhost:7003");
        }).AddPolicyHandler(_ => Policy<HttpResponseMessage>
                .Handle<Grpc.Core.RpcException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
        
        builder.Services.AddAuthentication()
            .AddKeycloakJwtBearer(serviceName: "keycloak", realm: "pocpro", options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = "account";
            });
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Open", b =>
                b.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });
        
        builder.Services.AddFastEndpoints();
        
        return builder.Build();
    }
    
    public static WebApplication AddPipeline(this WebApplication app)
    {
        _ = app.ConfigureDatabaseAsync();
        
        if (app.Environment.IsDevelopment())
        {
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