using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services
    .AddOcelot(builder.Configuration)
    .AddCacheManager(options => 
    {
        options.WithDictionaryHandle();
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

await app.UseOcelot();

app.Run();