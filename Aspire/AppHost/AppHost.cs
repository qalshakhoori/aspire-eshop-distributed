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

// Add projects and cloud-related services to the container.

var catalogSrv = builder.AddProject<Projects.Catalog>("catalog-srv")
  .WithReference(catalogDb)
  .WaitFor(catalogDb);

var basketSrv = builder.AddProject<Projects.Basket>("basket-srv")
  .WithReference(cache)
  .WithReference(catalogSrv)
  .WaitFor(cache);

builder.Build().Run();
