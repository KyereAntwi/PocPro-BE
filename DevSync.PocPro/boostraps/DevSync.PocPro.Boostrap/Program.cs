var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithExternalHttpEndpoints();

var accountsApi = builder.AddProject<Projects.DevSync_PocPro_Accounts_Api>("accounts-api")
    .WithReference(keycloak).WaitFor(keycloak);

var shopqpi = builder.AddProject<Projects.DevSync_PocPro_Shops_Api>("shops-api")
    .WithReference(keycloak).WaitFor(keycloak)
    .WithReference(accountsApi).WaitFor(accountsApi);

builder.AddProject<Projects.DevSync_PocPro_Gateway>("gateway")
    .WithReference(accountsApi).WaitFor(accountsApi)
    .WithReference(shopqpi).WaitFor(shopqpi);

builder.Build().Run();