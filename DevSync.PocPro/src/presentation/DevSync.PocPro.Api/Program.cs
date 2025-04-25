using DevSync.PocPro.Api.DI;

var builder = WebApplication.CreateBuilder(args);

var app = builder.AddServices().AddPipeline();

app.Run();