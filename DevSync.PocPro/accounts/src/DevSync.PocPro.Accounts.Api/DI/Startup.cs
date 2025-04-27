using DevSync.PocPro.Accounts.Api.Services;
using Scalar.AspNetCore;

namespace DevSync.PocPro.Accounts.Api.DI;

public static class Startup
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        var tenantDatabaseSettings = new TenantDatabaseSettings();
        builder.Configuration.GetSection("TenantDatabaseSettings").Bind(tenantDatabaseSettings);
        builder.Services.AddSingleton(tenantDatabaseSettings);
        
        builder.AddNpgsqlDbContext<AccountsDbContext>("PocProAccountsManagement");
        //builder.AddSeqEndpoint(connectionName: "seq");
        builder.AddRabbitMQClient(connectionName: "messaging");
        
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();
        
        builder.Services.AddScoped<IApplicationDbContext, AccountsDbContext>();
        builder.Services.AddScoped<ITenantServices, TenantServices>();
        
        builder.Services.AddAuthentication()
            .AddKeycloakJwtBearer(serviceName: "keycloak", realm: "account", options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = "account-api";
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
        app.ConfigureDatabaseAsync();
        
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
        
        app.UseCors("Open");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseFastEndpoints();
        app.UseHttpsRedirection();
        
        return app;
    }
}