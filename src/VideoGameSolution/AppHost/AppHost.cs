using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var secret = builder.AddParameter("super-secret", true);

var openApiViewer = builder.AddScalarApiReference(options =>
{
    options.PreferHttpsEndpoint = true;
    options.AllowSelfSignedCertificates = true;
}).WithLifetime(ContainerLifetime.Persistent);

var databaseServer = builder.AddPostgres("pg-server")
    .WithLifetime(ContainerLifetime.Persistent);

var gamesDb = databaseServer.AddDatabase("games-db");


var api = builder.AddProject<Projects.Games_Api>("games-api")
    .WithReference(gamesDb)
    .WithEnvironment("secret-thing", secret)
    .WaitFor(gamesDb);

openApiViewer.WithApiReference(api);

var app = builder.Build();
app.Run();
