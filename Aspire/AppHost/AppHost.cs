var builder = DistributedApplication.CreateBuilder(args);

// Add projects and cloud-related services to the container.

builder.AddProject<Projects.Catalog>("catalog");

builder.Build().Run();
