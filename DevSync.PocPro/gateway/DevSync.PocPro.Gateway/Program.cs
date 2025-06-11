using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

#if DEBUG
builder.Configuration.AddJsonFile("ocelot.development.json", optional: false, reloadOnChange: true);
#else
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
#endif

builder.Services
    .AddOcelot(builder.Configuration)
    .AddCacheManager(options => 
    {
        options.WithDictionaryHandle();
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("Open", b =>
        b.SetIsOriginAllowed(origin => 
                new Uri(origin).Host == "localhost")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseCors("Open");

await app.UseOcelot();

app.Run();