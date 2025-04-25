var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithExternalHttpEndpoints();

var postgres = builder
    .AddPostgres("postgres")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithDataVolume()
    .AddDatabase("PocproTenantsManagement");

var seq = builder.AddSeq("seq")
    .WithDataVolume()
    .ExcludeFromManifest()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("ACCEPT_EULA", "Y");

var messaging = builder
    .AddRabbitMQ("messaging")
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "admin")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "adminpassword")
    .WithManagementPlugin()
    .WithDataVolume();

builder
    .AddProject<Projects.DevSync_PocPro_Api>("devsync-pocpro-api")
    .WithReference(keycloak)
    .WithReference(postgres)
    .WithReference(seq)
    .WithReference(messaging)
    .WaitFor(keycloak)
    .WaitFor(postgres);

builder.Build().Run();
