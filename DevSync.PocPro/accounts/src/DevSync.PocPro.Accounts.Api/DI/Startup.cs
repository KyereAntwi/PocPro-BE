using DevSync.PocPro.Accounts.Api.EventsHandlers.Handlers;
using DevSync.PocPro.Accounts.Api.Features.Tenants.Grpc;
using DevSync.PocPro.Accounts.Api.Services;
using DevSync.PocPro.Shared.Domain.Middlewares;
using DevSync.PocPro.Shared.Domain.Utils;
using Scalar.AspNetCore;

namespace DevSync.PocPro.Accounts.Api.DI;

public static class Startup
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        var tenantDatabaseSettings = new TenantDatabaseSettings();
        builder.Configuration.GetSection("TenantDatabaseSettings").Bind(tenantDatabaseSettings);
        builder.Services.AddSingleton(tenantDatabaseSettings);
        
        var keycloakSettings = new KeycloakSettings();
        builder.Configuration.GetSection("KeycloakSettings").Bind(keycloakSettings);
        builder.Services.AddSingleton(keycloakSettings);
        
        builder.Services.AddDbContext<AccountsDbContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();
        
        builder.Services.AddScoped<IApplicationDbContext, AccountsDbContext>();
        builder.Services.AddHttpClient<IIdentityServices, IdentityServices>();

        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumer<RegisterUserLoginEventHandler>();
            
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
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["KeycloakSettings:Authority"];
                options.Audience = builder.Configuration["KeycloakSettings:Audience"];
                options.TokenValidationParameters.ValidateAudience = false;
                options.RequireHttpsMetadata = false;
            });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "account.api");
            });
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Open", b =>
                b.SetIsOriginAllowed(origin => 
                        //new Uri(origin).Host == "localhost" ||
                        new Uri(origin).Host == "https://devsyncproshopapi-ftgjb6bdf0b2d7av.uksouth-01.azurewebsites.net" ||
                        new Uri(origin).Host == "https://devsyncshopgateway-acdubue3e2eahkfs.uksouth-01.azurewebsites.net"
                        )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        builder.Services.AddGrpc();
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
                    .WithTitle("DevSync's PocPro Accounts API")
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }
        
        app.UseCustomExceptionHandler();
        app.UseCors("Open");
        
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseFastEndpoints();
        app.MapGrpcService<TenantsServicesImpl>();
        app.UseHttpsRedirection();
        
        return app;
    }
}