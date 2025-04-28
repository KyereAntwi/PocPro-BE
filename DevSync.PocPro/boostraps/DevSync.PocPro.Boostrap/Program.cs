var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithExternalHttpEndpoints();

var postgres = builder
    .AddPostgres("postgres", port: 5432)
    .WithDataVolume()
    .WithExternalHttpEndpoints()
    .AddDatabase("PocProAccountsManagement");

// var seq = builder.AddSeq("seq")
//     .WithDataVolume()
//     .ExcludeFromManifest()
//     .WithEndpoint(5341)
//     .WithLifetime(ContainerLifetime.Persistent)
//     .WithEnvironment("ACCEPT_EULA", "Y");

var messaging = builder
    .AddRabbitMQ("messaging", port: 5433)
    .WithManagementPlugin()
    .WithDataVolume()
    .WithExternalHttpEndpoints();

var accountsApi = builder.AddProject<Projects.DevSync_PocPro_Accounts_Api>("accounts-api")
    .WithReference(keycloak).WaitFor(keycloak)
    .WithReference(postgres).WaitFor(postgres)
    .WithReference(messaging)
    .WithExternalHttpEndpoints();
//.WithReference(seq);

builder.AddProject<Projects.DevSync_PocPro_Shops_Api>("shops-api")
    .WithReference(keycloak).WaitFor(keycloak)
    .WithReference(postgres).WaitFor(postgres)
    .WithReference(messaging)
    .WithReference(accountsApi).WaitFor(accountsApi);

builder.Build().Run();