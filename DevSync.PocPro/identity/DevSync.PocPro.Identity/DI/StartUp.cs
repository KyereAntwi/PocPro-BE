using DevSync.PocPro.Identity.Data;
using DevSync.PocPro.Identity.Pages.Admin.Clients;
using Duende.IdentityServer;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace DevSync.PocPro.Identity.DI;

public static class Startup
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName)));
        
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
                options.LicenseKey = builder.Configuration.GetValue<string>("DuendeIDS:LicenseKey");
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                        dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                        dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName));

                options.EnableTokenCleanup = true;
                options.RemoveConsumedTokens = true;
            })
            .AddAspNetIdentity<IdentityUser>();
        
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            //x.AddConsumer<RegisterUserLoginEventHandler>();
            
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
        builder.Services.AddScoped<ClientRepository>();
        
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication()
            .AddCookie()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = builder.Configuration["Google:ClientId"]!;
                options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
                options.CallbackPath = "/signin-google";
            })
            // .AddFacebook(options =>
            // {
            //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //     options.AppId = builder.Configuration["Facebook:AppId"];
            //     options.AppSecret = builder.Configuration["Facebook:AppSecret"];
            //     options.CallbackPath = "/signin-facebook";
            // })
            .AddLinkedIn(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = builder.Configuration["LinkedIn:ClientId"]!;
                options.ClientSecret = builder.Configuration["LinkedIn:ClientSecret"]!;
                options.CallbackPath = "/signin-linkedin";
            })
            .AddMicrosoftAccount(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = builder.Configuration["Microsoft:ClientId"]!;
                options.ClientSecret = builder.Configuration["Microsoft:ClientSecret"]!;
                options.CallbackPath = "/signin-microsoft";
            });
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Open", b =>
                b.SetIsOriginAllowed(origin =>
                    {
                        if (string.IsNullOrWhiteSpace(origin))
                            return false;
                        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                            return false;
                        return uri.Host == "localhost" || 
                               uri.Host == "https://devsyncaccountsapi-ccguashuc8a5gpfs.uksouth-01.azurewebsites.net";
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });
        
        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseCors("Open");
        app.UseStaticFiles();
        app.UseRouting();
        
        app.UseIdentityServer();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapRazorPages();
        app.MapControllers();
        
        return app;
    }
}