using Alba;
using System;
using System.Collections.Generic;
using System.Text;

namespace Games.Tests;

[Collection("GamesSystemTests")]
public class GoodApiTest(TestFixture fixture)
{
    [Fact]
    public async Task CanGetStatus()
    {

        var expectedResponseBody = new StatusResponse
        {
            Message = "All is good",
            UpTimeInHours = 42
        };
        var response = await fixture.Host.Scenario(api =>
        {
            api.Get.Url("/status");
            api.StatusCodeShouldBeOk();
        });

        var body = await response.ReadAsJsonAsync<StatusResponse>();

        Assert.NotNull(body);

        Assert.Equal(expectedResponseBody, body);
    }
}
