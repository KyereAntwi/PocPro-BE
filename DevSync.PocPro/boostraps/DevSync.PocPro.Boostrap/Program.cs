var builder = DistributedApplication.CreateBuilder(args);

var identity = builder
    .AddProject<Projects.DevSync_PocPro_Identity>("identity")
    .WithExternalHttpEndpoints();

var accountsApi = builder.AddProject<Projects.DevSync_PocPro_Accounts_Api>("accounts-api")
    .WithReference(identity)
    .WaitFor(identity);

var shopqpi = builder.AddProject<Projects.DevSync_PocPro_Shops_Api>("shops-api")
    .WithReference(identity).WaitFor(identity)
    .WithReference(accountsApi).WaitFor(accountsApi);

builder.AddProject<Projects.DevSync_PocPro_Gateway>("gateway")
    .WithReference(accountsApi).WaitFor(accountsApi)
    .WithReference(shopqpi).WaitFor(shopqpi)
    .WithExternalHttpEndpoints();

builder.Build().Run();