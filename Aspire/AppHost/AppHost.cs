var builder = DistributedApplication.CreateBuilder(args);

// Backing services
var postgres =
  builder.AddPostgres("postgres")
  .WithPgAdmin()
  // .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalog-db");

var cache = builder.AddRedis("cache")
  .WithRedisInsight()
  // .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder
  .AddRabbitMQ("rabbitmq")
  .WithManagementPlugin()
  // .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder.AddKeycloak("keycloak", 8080)
  // .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent);

if (builder.ExecutionContext.IsRunMode)
{
  // Use a data volume in run mode to persist data across restarts
  postgres.WithDataVolume();
  cache.WithDataVolume();
  rabbitmq.WithDataVolume();
  keycloak.WithDataVolume();
}

var ollama = builder.AddOllama("ollama", 11434)
  .WithDataVolume()
  .WithLifetime(ContainerLifetime.Persistent)
  .WithOpenWebUI();

var llama = ollama.AddModel("llama3.2");
var embeddings = ollama.AddModel("all-minilm");

// Add projects and cloud-related services to the container.
var catalogSrv = builder.AddProject<Projects.Catalog>("catalog-srv")
  .WithReference(catalogDb)
  .WithReference(rabbitmq)
  .WithReference(llama)
  .WithReference(embeddings)
  .WaitFor(catalogDb)
  .WaitFor(rabbitmq)
  .WaitFor(llama)
  .WaitFor(embeddings);

var basketSrv = builder.AddProject<Projects.Basket>("basket-srv")
  .WithReference(cache)
  .WithReference(rabbitmq)
  .WithReference(catalogSrv)
  .WithReference(keycloak)
  .WaitFor(cache)
  .WaitFor(rabbitmq)
  .WaitFor(keycloak);


builder.AddProject<Projects.WebApp>("webapp")
  .WithExternalHttpEndpoints()
  .WithReference(cache)
  .WithReference(catalogSrv)
  .WithReference(basketSrv)
  .WaitFor(catalogSrv)
  .WaitFor(basketSrv);

builder.Build().Run();
