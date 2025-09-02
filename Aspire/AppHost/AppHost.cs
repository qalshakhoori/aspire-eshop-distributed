var builder = DistributedApplication.CreateBuilder(args);

// Backing services
var postgres =
  builder.AddPostgres("postgres")
  .WithPgAdmin()
  .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalog-db");

var cache = builder.AddRedis("cache")
  .WithRedisInsight()
  .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder
  .AddRabbitMQ("rabbitmq")
  .WithManagementPlugin()
  .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder.AddKeycloak("keycloak", 8080)
  .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

// Add projects and cloud-related services to the container.
var catalogSrv = builder.AddProject<Projects.Catalog>("catalog-srv")
  .WithReference(catalogDb)
  .WithReference(rabbitmq)
  .WaitFor(catalogDb)
  .WaitFor(rabbitmq);

var basketSrv = builder.AddProject<Projects.Basket>("basket-srv")
  .WithReference(cache)
  .WithReference(rabbitmq)
  .WithReference(catalogSrv)
  .WithReference(keycloak)
  .WaitFor(cache)
  .WaitFor(rabbitmq)
  .WaitFor(keycloak); 

builder.Build().Run();
