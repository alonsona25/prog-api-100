
using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Software.Api.Vendors;
using System.ComponentModel;
using System.Security.Claims;

namespace Software.Tests.Vendors;

[Trait("Level","UnitIntegration")]
[Collection("UnitIntegrationTests")]
public class AddingAVendor(UnitIntegrationFixture fixture)
{

    [Fact]
    public async Task ValidatesRequest()
    {
        var vendorCreateModel = new VendorCreateModel
        {
            Name = "123" + Guid.NewGuid().ToString(),
            Url = "testvendor.com",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                Email = "john.doe@testvendor.com"
            }
        };

        // Post the vendor create model to the /vendors endpoint and expect a 201 Created response

        var postResponseMessage = await fixture.Host.Scenario(api =>
        {
            api.WithClaim(new Claim(ClaimTypes.Name, "jill@company.com"));
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.Post.Json(vendorCreateModel).ToUrl("/vendors");
            api.StatusCodeShouldBe(400);
        });
    }
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
            api.WithClaim(new Claim(ClaimTypes.Name, "jill@company.com"));
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
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
