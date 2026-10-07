# Developer Testing

Open the SoftwareCenter solution in Visual Studio. 

We will add an XUnit Test project.

In the Solution Explorer window, right click on the solution "SoftwareCenter" and select "Add new project..."

Change the project type filter to "XUnit" and select the XUnit test project template (with C#).

Give the project the name "Software.Tests", and accept the defaults for all other options.

## Configuring the Test Project

- [ ] In the `Software.Tests` project, delete the UnitTest1.cs file
- [ ] In the Solution Explorer, right click on "Dependencies" under the Software.Tests project and select "Add project reference..."
- [ ] Select (check) only the Software.Api project and close the dialog.
- [ ] Add the following NuGet packages to the test project: Alba, NSubstitute, Microsoft.Extensions.TimeProvider.Testing and Testcontainers.PostgreSql
    - Right click on the Software.Tests project and select "Manage Nuget Packages", use the browse tab to search and install.

## Create test fixtures

Since we are writing System Tests, we will create stateful fixtures for our tests. We will start with a fixture for just SystemTests, but also have a fixture for unit integration tests.

- [ ] In the test project create a directory called "Fixtures"
- [ ] In this directory, create a class called "SystemTestFixture.cs"
- [ ] Add the following fixture to that file:

```cs
using Alba;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Software.Tests;

[CollectionDefinition("SoftwareSystemTests")]
public class SoftwareTestCollection : ICollectionFixture<TestFixture> { }
public class TestFixture : IAsyncLifetime
{

    public IAlbaHost Host { get; set; } = null!;
    private PostgreSqlContainer _pgContainer = null!;


    public async Task InitializeAsync()
    {
        _pgContainer = new PostgreSqlBuilder("postgres:17").Build();
     
        await _pgContainer.StartAsync();
        Host = await AlbaHost.For<Program>(options =>
        {
            options.UseSetting("ConnectionStrings:software-db", _pgContainer.GetConnectionString());
            options.ConfigureServices(svc =>
            {
                ConfigureServices(svc);
            });
            options.ConfigureTestServices(svc =>
            {
                ConfigureTestServices(svc);
            });
        });
    }
    public async Task DisposeAsync()
    {
        await _pgContainer.StopAsync();
        await Host.StopAsync();
    }
    protected void ConfigureTestServices(IServiceCollection services)
    {
        // Add any additional test-specific services here
    }
    protected void ConfigureServices(IServiceCollection services)
    {
        // Add any additional services here
    }
}
```

## Create the first test

We will write a system test to verify we can add vendors.

- [ ] In the test project, create `Vendors/CanAddAVendor.cs`

Add the following test - which is a review of the example from yesterday:

```cs

using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Software.Api.Vendors;

namespace Software.Tests.Vendors;

[Collection("SoftwareSystemTests")]
public class AddingAVendor(TestFixture fixture)
{
    [Fact]
    public async Task UsingPostToCreateAVendor()
    {
        var vendorCreateModel = new VendorCreateModel
        {
            Name = "Test Vendor",
            Url = "https://testvendor.com",
            PointOfContact = new VendorPointOfContact {
                Name = "John Doe",
                Email = "john.doe@testvendor.com"
            }
        };

        // Post the vendor create model to the /vendors endpoint and expect a 201 Created response

        var postResponseMessage = await fixture.Host.Scenario(api =>
        {
            api.Post.Json(vendorCreateModel).ToUrl("/vendors");
            api.StatusCodeShouldBe(201);
        });

        // Read the response as a VendorDetailsModel
        var createdVendor = postResponseMessage.ReadAsJson<VendorDetailsModel>();

        Assert.NotNull(createdVendor);
        Assert.Equal(vendorCreateModel.Name, createdVendor.Name);
        Assert.Equal(vendorCreateModel.Url, createdVendor.Url);
        Assert.Equal(vendorCreateModel.PointOfContact, createdVendor.PointOfContact);


        // Get the location header from the response and use it to retrieve the vendor details
        var location = postResponseMessage.Context.Response.Headers.Location.First();
        Assert.NotNull(location);
        var path = new Uri(location).LocalPath; // the full location will include the scheme and host, but we only want the path for the next request

        // Retrieve the vendor details using the location header
        var getResponseMessage = await fixture.Host.Scenario(api =>
        {
            api.Get.Url(path);
            api.StatusCodeShouldBeOk();
        });
        var retrievedVendor = getResponseMessage.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(retrievedVendor);
        Assert.Equal(createdVendor, retrievedVendor);
    }
}
```

- [ ] Run the test and ensure it passes. (It will take a while, particularly the first time, and make sure Docker Desktop is running!)

## Add a true "gray box" test

We will add a new test class to the Vendors directory called "GettingVendors.cs". 

We will write two tests (Facts). The first one will directly access the database and add a vendor with a fixed ID, and then see if we can retrieve it from the database.

The second test will clear the `VendorEntity` table completely, and then add in 32 new Vendors to test our pagination.

```cs

using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Software.Api.Vendors;

namespace Software.Tests.Vendors;

[Collection("SoftwareSystemTests")]
public class GettingVendors(TestFixture fixture)
{
    [Fact]
    public async Task CanRetrieveAVendorById()
    {

        using var sp = fixture.Host.Services.CreateScope();
        var session = sp.ServiceProvider.GetRequiredService<IDocumentSession>();
        var jeffsBirthday = new DateTimeOffset(1969, 4, 20, 23, 59, 59, TimeSpan.Zero);
        var vendorEntity = new VendorEntity
        {
            Id = Guid.NewGuid(),
            Name = "Goliath",
            Url = "https://testvendor.com",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                Email = "John@aol.com"

            },
            CreatedAt = jeffsBirthday,
        };
        session.Store(vendorEntity);
        await session.SaveChangesAsync();

        var getResponseMessage = await fixture.Host.Scenario(api =>
        {
            api.Get.Url($"/vendors/{vendorEntity.Id}");
            api.StatusCodeShouldBeOk();
        });

        var retrievedVendor = getResponseMessage.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(retrievedVendor);
        Assert.Equal(vendorEntity.Id, retrievedVendor.Id);
        Assert.Equal(vendorEntity.Name, retrievedVendor.Name);
    }

    [Fact]
    public async Task CanGetAListOfVendors()
    {
        using var sp = fixture.Host.Services.CreateScope();
        var session = sp.ServiceProvider.GetRequiredService<IDocumentSession>();
        await session.DocumentStore.Advanced.Clean.DeleteAllDocumentsAsync();

        foreach (var item in Enumerable.Range(1, 32))
        {
            var vendorEntity = new VendorEntity
            {
                Id = Guid.NewGuid(),
                Name = $"Vendor {item}",
                Url = $"https://vendor{item}.com",
                PointOfContact = new VendorPointOfContact
                {
                    Name = $"Contact {item}",
                    Email = $"contact{item}@vendor{item}.com"
                }
            };
            session.Store(vendorEntity);
        }
        await session.SaveChangesAsync();

        var getResponseMessage = await fixture.Host.Scenario(api =>
        {
            api.Get.Url($"/vendors");
            api.StatusCodeShouldBeOk();
        });

        var retrievedVendors = getResponseMessage.ReadAsJson<VendorSummary>();
        Assert.NotNull(retrievedVendors);
        Assert.Equal(10, retrievedVendors.Vendors.Count);

        var secondPageResponseMessage = await fixture.Host.Scenario(api =>
        {
            api.Get.Url($"/vendors?page=2");
            api.StatusCodeShouldBeOk();
        });

        Assert.Equal(10, secondPageResponseMessage.ReadAsJson<VendorSummary>().Vendors.Count);

        var lastPageResponseMessage = await fixture.Host.Scenario(api =>
        {
            api.Get.Url($"/vendors?page=4");
            api.StatusCodeShouldBeOk();
        });

        Assert.Equal(2, lastPageResponseMessage.ReadAsJson<VendorSummary>().Vendors.Count);


    }
}
```

## Summary

We tested adding a vendor, retrieving a vendor, and our vendor list pagination.

- [ ] How would you make the tests we wrote more "confidence inspiring"?
- [ ] What tests are we missing, per the requirements?
= [ ] What is Jeff's birthday? Add it to your calendar with a reminder to send him best wishes. ;)


