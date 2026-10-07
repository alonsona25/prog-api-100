using Marten;

var builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDataSource("games-db");

var reviewsConnectionString = builder.Configuration.GetConnectionString("reviews-db")
    ?? throw new Exception("Cannot start without connection string!");

Console.WriteLine($"Using this connnection string { reviewsConnectionString}");

builder.AddServiceDefaults();

// Add services to the container.

// add a service that provides the session (IDocumentSession)

builder.Services.AddMarten(options =>
{
    
}).UseNpgsqlDataSource()
.UseLightweightSessions();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<TimeProvider>((_) => TimeProvider.System);
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapGet("/status", () =>
{
    return Results.Ok(new StatusResponse
    {
        Message = "All is good",
        UpTimeInHours = 42
    });
});
app.Run();

public record StatusResponse
{
    public string Message { get; set; } = string.Empty;
    public int UpTimeInHours { get; set; }
}