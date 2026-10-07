using Marten;
using Marten.Schema;
using Software.Api.Catalog;
using Software.Api.Vendors;
using Weasel.Postgresql.Tables;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddValidation(); // minimal APIs
builder.Services.AddProblemDetails(); // use the problem details response type for unhandled exceptions, etc.

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    // override here, but usually everything is in configured in appsettings.json
});

builder.Services.AddAuthorizationBuilder().AddPolicy("SoftwareCenterManager", policy =>
{
    policy.RequireRole("SoftwareCenter");
    policy.RequireRole("Manager");
});


builder.AddNpgsqlDataSource("software-db");

builder.Services.AddMarten(options =>
{
    options.RegisterDocumentType<VendorEntity>();
  options.Schema.For<VendorEntity>().Index(v => v.NormalizedName, idx => {
      idx.Casing = ComputedIndex.Casings.Lower;
      idx.IsUnique = true;
  });
}).UseLightweightSessions()
.UseNpgsqlDataSource()
.ApplyAllDatabaseChangesOnStartup();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<INormalizeVendorNames, Normalizer>();
builder.Services.AddCatalogServices();
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapCatalog();


app.Run();
