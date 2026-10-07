# Create the Software Solution, API and Hosting Environment

## Open Visual Studio and create a new solution

- Open Visual Studio 2026
- Click "Create new Project"
- In the filter drop down list in the third column ("All project types") select "Aspire"
- Find and select the "Aspire AppHost" template (with C#)
- Click "Next"
- The project name should be "AppHost"
- The location should be `c:\Users\student\class\src`
- Set the Solution name to "SoftwareSolution"
- Do *not* check "Place solution and project in the same directory"
- Click "Next"
- Leave the Framework as .NET 10.0
- Check the Configure for HTTPS check box
- Leave the "Use the .dev.localhost..." unchecked
- Click Create
## Add service defaults
- [ ] In the Visual Studio File menu, select "Add a new project"
- [ ] Select "Aspire Service Defaults" (C#)
- [ ] Click "Next"
- [ ] Set the name to "ServiceDefaults"
- [ ] Leave the location as is
- [ ] Click "Next"
- [ ] Leave the Framework select at .NET 10
- [ ] Click "Create"

## Add the API Project
- [ ] In the Visual Studio File menu, select "Add a new project"
- [ ] Change the project type filter (third column, currently "Aspire") to "API"
- [ ] Select "ASP.NET Core Web API" (C#)
- [ ] Click "Next"
- [ ] Change the name to Software.Api and leave the location as is.
- [ ] Click "Next"
- [ ] On the Additional Information step:
	- [ ] Framework: .NET 10.0 (Long Term Support)
	- [ ] Authentication type: none
	- [ ] Check "Configure for HTTPS"
	- [ ] Uncheck "Enable container support"
	- [ ] Check "Enable OpenAPI support"
	- [ ] Uncheck "Do not use top-level statements"
	- [ ] Check "Use controllers"
	- [ ] Check "Enlist in Aspire orchestration"
- [ ] Click "Create"

## AppHost configuration
We will configure the AppHost dashboard to run on a stable HTTP port.

- [ ] Open the file "launchSettings.json" in the AppHost's Properties folder using the solution explorer
- [ ] Remove the `http` section
- [ ] Change the `https` section to use `https://localhost:8080` for the `applicationUrl`
- [ ] Remove the `http` address from that url.
- [ ] Save and close the file

<details>
<summary>Your final AppHost  `launchSettings.json` file</summary>

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:8080",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_ENVIRONMENT": "Development",
        "ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL": "https://localhost:21135",
        "ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL": "https://localhost:22177"
      }
    }
  }
}
```

</details>

## Software.Api configuration
We will do a similar move for the Software.Api project

- [ ] Open the `launchSettings.json` file for the Software.Api project in the solution file.
- [ ] Change the https port to 1337, and remove the http configuration
- [ ] Save and close the file

<details>
<summary>Your final Software.Api `launchSettings.json` file</summary>

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
   
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:1337",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

</details>

## Start the AppHost dashboard

- [ ] Start the application by hitting `Ctrl+F5`
- [ ] Your dashboard should load in the browser at https://localhost:8080 and it should show your Software.Api (`software-api`) as running. 

## Add Scalar OpenAPI support to the dashboard
So that we can interact with the API through the generated OpenAPI documentation, we will add Scalar to our AppHost project and have it load our OpenApi spec from our API.

In Visual Studio:
- [ ] Right click on the AppHost project in the solution explorer and select "Manage Nuget Packages".
- [ ] Select the "Browse" tab and search for "Scalar.Aspire". Select it from the list
- [ ] In the right pane, select "Install"
- [ ] In the `AppHost.cs` file, replace the existing code with the following:

```c#
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var scalar = builder.AddScalarApiReference(options =>
{
    options.PreferHttpsEndpoint = true;
    options.AllowSelfSignedCertificates = true;
});

var softwareApi =builder.AddProject<Projects.Software_Api>("software-api");

scalar.WithApiReference(softwareApi);

builder.Build().Run();

```

- [ ] Run your project again (`Ctrl+F5`) and wait for Scalar to start on the dashboard. When it is started, click the link "Scalar API Reference"
- [ ] Your Software.API endpoint for the weather forecast should be shown. A Software catalog has no need for a weather forecast.

## Remove the weather forecast fake endpoint from the Software.Api

- [ ] In the `Software.Api` remove the `WeatherForecast.cs` file and delete the entire `Controllers` folder.

## Add the Vendors endpoints and models

We will start with the ability to add, retrieve, and list vendors. In the `Software.Api` project:

- [ ] Add a new directory called "Vendors"
- [ ] In that directory, add three C# Classes: (you can right-click on the folder and select "Add new item...")
	- [ ] `VendorsController.cs`
	- [ ] `Models.cs`
	- [ ] `Data.cs`
- [ ] Add the model types. These types will represent the data coming into the API from requests, and leaving the API in the form of HTTP responses. 

In `Vendors/Models.cs`:

```cs
using System.ComponentModel.DataAnnotations;

namespace Software.Api.Vendors;

public record VendorDetailsModel
{
    public Guid Id { get; set; }
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required VendorPointOfContact PointOfContact { get; init; }
}

public record VendorPointOfContact
{
    public required string Name { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public record VendorCreateModel
{
    [MinLength(5), MaxLength(100)]
    public required string Name { get; init; }
    [Url]
    public required string Url { get; init; }
    public required VendorPointOfContact PointOfContact { get; init; }
}

public record VendorSummaryModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }
}

public record VendorSummary
{
    public required IReadOnlyList<VendorSummaryModel> Vendors { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
}
```

In the `data.cs` file, add an Entity type for the vendors. This will be the type that we save in the database.

```cs
namespace Software.Api.Vendors;

public class VendorEntity
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required VendorPointOfContact PointOfContact { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
```

## Implement a "fake" version of the controller

In the `VendorsController.cs` paste the following code:

```cs
using Microsoft.AspNetCore.Mvc;


namespace Software.Api.Vendors;

[ApiController]
[Route("vendors")]
public class VendorsController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<VendorDetailsModel>> CreateVendor([FromBody] VendorCreateModel model)
    {
        // Simulate creating a vendor and returning the details
        var newVendor = new VendorDetailsModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Url = model.Url,
            PointOfContact = model.PointOfContact
        };
        // In a real application, you would save the new vendor to a database here
        return CreatedAtAction(nameof(GetVendorById), new { id = newVendor.Id }, newVendor);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VendorDetailsModel>> GetVendorById(Guid id)
    {
        // Simulate retrieving a vendor by ID
        var vendor = new VendorDetailsModel
        {
            Id = id,
            Name = "Sample Vendor",
            Url = "https://example.com",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890"
            }
        };
        // In a real application, you would retrieve the vendor from a database here
        return Ok(vendor);
    }
    [HttpGet]
    public async Task<ActionResult<VendorSummary>> GetVendors([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Simulate retrieving a list of vendors with pagination
        var vendors = new List<VendorSummaryModel>();
        for (int i = 0; i < pageSize; i++)
        {
            vendors.Add(new VendorSummaryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Vendor {((page - 1) * pageSize) + i + 1}",
                Url = $"https://example.com/vendor{((page - 1) * pageSize) + i + 1}"
            });
        }
        var summary = new VendorSummary
        {
            Vendors = vendors,
            Page = page,
            PageSize = pageSize
        };
        // In a real application, you would retrieve the vendors from a database here
        return Ok(summary);
    }

}

```

This is "fake", but will allow you to experiment with making various requests in the Scalar user interface. Run the application, go to the Scalar API endpoint, and try to test out the various endpoints:

- Creating a Vendor
- Getting a paginated list of vendors
- Getting a specific vendor
