using DevSync.PocPro.Shops.Api.Data;
using FastEndpoints;
using Scalar.AspNetCore;

namespace DevSync.PocPro.Shops.Api.DI;

public static class Startup
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<MainShopDbContext>("PocProAccountsManagement");
        //builder.AddSeqEndpoint(connectionName: "seq");
        builder.AddRabbitMQClient(connectionName: "messaging");
        
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();
        
        builder.Services.AddAuthentication()
            .AddKeycloakJwtBearer(serviceName: "keycloak", realm: "account", options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = "shops-api";
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
        if (app.Environment.IsDevelopment())
        {
            //app.ConfigureDatabaseAsync();
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