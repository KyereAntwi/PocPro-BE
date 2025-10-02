using DevSync.PocPro.Accounts.Api.CQRS.Handlers;
using DevSync.PocPro.Accounts.Api.Features.Tenants.Grpc;
using DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CheckExistingUniqueIdentifier;
using DevSync.PocPro.Accounts.Api.Services;
using DevSync.PocPro.Shared.Domain.Middlewares;
using Scalar.AspNetCore;
using GetTenantDetailsResponse = DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetTenantDetails.GetTenantDetailsResponse;
using CreateSubAccountRequest = DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateSubAccount.CreateSubAccountRequest;

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
        
        var apiAuthentication = new ApiAuthentication();
        builder.Configuration.GetSection("ApiAuthentication").Bind(apiAuthentication);
        builder.Services.AddSingleton(apiAuthentication);
        
        var messageBroker = new MessageBroker();
        builder.Configuration.GetSection("MessageBroker").Bind(messageBroker);
        builder.Services.AddSingleton(messageBroker);
        
        builder.Services.AddDbContext<AccountsDbContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();

        builder.Services.AddScoped<IQueryHandler<GetAllTenantsQuery, List<GetTenantDetailsResponse>>, GetAllTenantsQueryHandler>();
        builder.Services
            .AddScoped<IQueryHandler<CheckExistingUniqueIdentifierQuery, CheckExistingUniqueIdentifierResponse>,
                CheckExistingUniqueIdentifierQueryHandler>();
        builder.Services
            .AddScoped<Shared.Domain.CQRS.ICommandHandler<CreateATenantRequest, CreateATenantResponse>,
                CreateATenantCommandHandler>();
        builder.Services
            .AddScoped<Shared.Domain.CQRS.ICommandHandler<CreateSubAccountRequest, Guid>, CreateSubAccountCommandHandler>();
        
        builder.Services.AddScoped<IApplicationDbContext, AccountsDbContext>();
        builder.Services.AddHttpClient<IIdentityServices, IdentityServices>();

        builder.Services.AddSingleton<RabbitMqConnectionFactory>();
        builder.Services.AddSingleton<EventPublisher>();
        
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

        builder.Services.AddGrpcClient<PaymentService.PaymentServiceClient>(options =>
        {
            options.Address = new Uri("https://devsync-paymentserviceapi.azurewebsites.net");
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