using System;
using System.Collections.Generic;
using System.Text;

namespace Software.Tests.Vendors;

[Trait("Level", "SystemTest")]
public class UnauthenticatedTests(UnAuthenticatedTestFixture fixture) : IClassFixture<UnAuthenticatedTestFixture>
{

    [Fact]
    public async Task CannotGetVendorsIfNotAuthenticated()
    {
        var getResponseMessage = await fixture.Host.Scenario(api =>
        {
            api.Get.Url("/vendors");
            api.StatusCodeShouldBe(401);
        });
    }
}
