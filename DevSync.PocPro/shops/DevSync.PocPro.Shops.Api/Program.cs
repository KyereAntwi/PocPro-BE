using DevSync.PocPro.Shops.Api.DI;
using DevSync.PocPro.Shops.WorkerService.DI;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .AddServices()
    .AddPipeline()
    .AddJobScheduler();

app.Run();