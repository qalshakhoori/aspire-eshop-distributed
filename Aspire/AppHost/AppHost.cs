var builder = DistributedApplication.CreateBuilder(args);

// Backing services
var postgres =
  builder.AddPostgres("postgres")
  .WithPgAdmin()
  .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalog-db");

// Add projects and cloud-related services to the container.

var catalogSrv = builder.AddProject<Projects.Catalog>("catalog-srv")
  .WithReference(catalogDb)
  .WaitFor(catalogDb);

builder.Build().Run();
