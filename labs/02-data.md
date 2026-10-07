# Add Persistence

In this lab we will add a database to our AppHost environment, give our API a reference to that database, install a database library, and make our API more *real*.

## Add Postgres to the AppHost

- [ ] In Visual Studio, right-click on the AppHost project and select "Manage Nuget Packages"
- [ ] On the "browse" tab, search for `Aspire.Hosting.PostgreSQL` and install it.
- [ ] Update your `AppHost.cs` to look like the following:

```cs
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var scalar = builder.AddScalarApiReference(options =>
{
    options.PreferHttpsEndpoint = true;
    options.AllowSelfSignedCertificates = true;
});

var pg = builder.AddPostgres("pg")
    .WithLifetime(ContainerLifetime.Persistent);

var softwareDb = pg.AddDatabase("software-db");

var softwareApi = builder.AddProject<Projects.Software_Api>("software-api")
    .WithReference(softwareDb)
    .WaitFor(softwareDb);

scalar.WithApiReference(softwareApi);

builder.Build().Run();

```

- [ ] Restart the app, and wait for Postgres to start. 

## Add configuration for the Software.Api

Right-click on the Software.Api project and add the following NuGet packages:

- [ ] `Aspire.NpgSql` (This is just a postgres connection library and really doesn't have anything to do with Aspire.)
- [ ] `Marten.AspNetCore` (A library that makes it easy to work with Postgres)

In the `Software.Api`'s `Program.cs` file, add the following right after the comment to "// Add services to the container":

```cs
builder.AddNpgsqlDataSource("software-db");

builder.Services.AddMarten(options =>
{

}).UseLightweightSessions()
.UseNpgsqlDataSource();
```

Note: You will have to add a `using Marten;` directive at the top of `Program.cs`

## Implement the `POST` method

On the `VendorsController.cs`, we will implement the POST method. 

- [ ] You will need two services provided, one for the `SystemTime`, and the other for the `IDocumentSession` provided by Marten.
- [ ] You will create an store an instance of the `VendorEntity` in the database
- [ ] You will *map* the VendorEntity to a `VendorDetailsModel` and return a 201 Status Code.

```cs
    [HttpPost]
    public async Task<ActionResult<VendorDetailsModel>> CreateVendor([FromBody] VendorCreateModel model, [FromServices] TimeProvider clock, [FromServices] IDocumentSession session)
    {
    
        var vendorEntity = new VendorEntity
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Url = model.Url,
            PointOfContact = model.PointOfContact,
            CreatedAt = clock.GetUtcNow(),
            CreatedBy = User.Identity?.Name ?? "Unknown"
        };

        session.Store(vendorEntity);
        await session.SaveChangesAsync();

        var newVendor = new VendorDetailsModel
        {
            Id = vendorEntity.Id,
            Name = vendorEntity.Name,
            Url = vendorEntity.Url,
            PointOfContact = vendorEntity.PointOfContact
        };

        return CreatedAtAction(nameof(GetVendorById), new { id = newVendor.Id }, newVendor);
    }
```

Run the application and use Scalar to add a new Vendor.

## Get a list of Vendors

Replace the code for the `HttpGet` with the following:

```cs
 [HttpGet]
 public async Task<ActionResult<VendorSummary>> GetVendors([FromServices] IDocumentSession session, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
 {

     var vendors = await session.Query<VendorEntity>()
         .OrderBy(v => v.Name)
         .Skip((page - 1) * pageSize)
         .Take(pageSize)
         .Select(v => new VendorSummaryModel
         {
             Id = v.Id,
             Name = v.Name,
             Url = v.Url
         })
         .ToListAsync();

     var summary = new VendorSummary
     {
         Vendors = vendors,
         Page = page,
         PageSize = pageSize
     };
     return Ok(summary);
 }
```

Test it out in the Scalar UI.

## Implement Get by Id

We will implement our `GetVendorByIdAsync` method as follows. Note the addition of the route guard on the `id` parameter. This tells the web server to *not* call this method if that `id` parameter does not *look like* a GUID (uuid).

```cs
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VendorDetailsModel>> GetVendorById(Guid id, [FromServices] IDocumentSession session)
    {

        var response = await session.Query<VendorEntity>()
            .Select(v => new VendorDetailsModel
            {
                Id = v.Id,
                Name = v.Name,
                Url = v.Url,
                PointOfContact = v.PointOfContact
            })
            .SingleOrDefaultAsync(v => v.Id == id);
  
       return response switch
       {
           null => NotFound(),
           _ => Ok(response)
       };   
    }
```