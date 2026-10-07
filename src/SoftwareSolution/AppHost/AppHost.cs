using System.ComponentModel;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var scalar = builder.AddScalarApiReference(options =>
{
    options.PreferHttpsEndpoint = true;
    options.AllowSelfSignedCertificates = true;
}).WithLifetime(ContainerLifetime.Persistent);

var pg = builder.AddPostgres("pg")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();
    //.WithVolume("./vendor-db-data");

var softwareDb = pg.AddDatabase("software-db");

var softwareApi = builder.AddProject<Projects.Software_Api>("software-api")
    .WithReference(softwareDb)
    .WaitFor(softwareDb)
    .WithTerminal();

scalar.WithApiReference(softwareApi);

builder.Build().Run();