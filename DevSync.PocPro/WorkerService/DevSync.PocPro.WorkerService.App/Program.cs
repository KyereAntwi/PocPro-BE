using DevSync.PocPro.WorkerService.DI;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder
    .AddServices()
    .AddJobScheduler();

app.Run();