using DevSync.PocPro.Shops.Api.Services;
using DevSync.PocPro.Shops.OrdersModule.DI;
using DevSync.PocPro.Shops.PointOfSales.DI;

namespace DevSync.PocPro.Shops.Api.DI;

public static class Startup
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddStockModule(builder.Configuration);
        builder.Services.RegisterOrderModule();
        builder.Services.AddPosDependencies();
        
        builder.Services.AddTransient<ITenantServices, TenantServices>();
        builder.Services.AddScoped<ITenantRegistrationServices, TenantRegistrationServices>();
        
        builder.AddNpgsqlDbContext<MainShopTemplateDbContext>("PocProAccountsManagement");
        //builder.AddSeqEndpoint(connectionName: "seq");
        builder.AddRabbitMQClient(connectionName: "messaging");
        
        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<GenerateTenantDatabaseEventHandler>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("messaging");
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