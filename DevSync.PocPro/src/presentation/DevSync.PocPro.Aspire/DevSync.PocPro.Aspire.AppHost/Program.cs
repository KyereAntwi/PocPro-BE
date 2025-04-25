var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DevSync_PocPro_Api>("devsync-pocpro-api");

builder.Build().Run();
