
using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Software.Api.Vendors;

namespace Software.Tests.Vendors;

[Trait("Level", "SystemTest")]
[Collection("SoftwareSystemTests")]
public class GettingVendors(TestFixture fixture)
{
    [Fact]
    public async Task CanRetrieveAVendorById()
    {

        using var sp = fixture.Host.Services.CreateScope(); // 
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

    [Fact(Skip = "Skipping this test for now")]
    public async Task CanGetAListOfVendors()
    {
        using var sp = fixture.Host.Services.CreateScope();
        var session = sp.ServiceProvider.GetRequiredService<IDocumentStore>();
        await session.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(VendorEntity));

        var vendorsToAdd = new List<VendorEntity>();
        foreach (var item in Enumerable.Range(1, 32))
        {
            var id = new Guid();
            var vendorEntity = new VendorEntity
            {
                Id = id,
                Name = $"{id.ToString() + DateTime.Now.Microsecond.ToString()}",
                Url = $"https://vendor{item}.com",
                PointOfContact = new VendorPointOfContact
                {
                    Name = $"Contact {item}",
                    Email = $"contact{item}@vendor{item}.com"
                }
            };
            vendorsToAdd.Add(vendorEntity);
        }
        await session.BulkInsertAsync(vendorsToAdd, batchSize: 100);

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