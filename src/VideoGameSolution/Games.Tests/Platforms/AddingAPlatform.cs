
using Alba;
using Games.Api.Platforms;

namespace Games.Tests.Platforms;

[Collection("GamesSystemTests")]
public class AddingAPlatform(TestFixture fixture)
{
    [Fact]
    public async Task Posting()
    {
        // send a request, get a response. etc.
        var responseMessage = await fixture.Host.Scenario(api =>
        {
            api.Post.Json(new PlatformCreateRequest { Name = "XBox" }).ToUrl("/platforms");
            api.StatusCodeShouldBeOk();
        });

        var postResponseBody = await responseMessage.ReadAsJsonAsync<PlatformDetailsResponse>();
        //check some stuff
        Assert.Equal("XBox", postResponseBody.Name);

        var getResponse = await fixture.Host.Scenario(api =>
        {
            api.Get.Url("/platforms");
            // if it isn't a 200 it will fail, you don't have to check
        });

        var listOfAllPlatforms = await getResponse.ReadAsJsonAsync<List<PlatformDetailsResponse>>();


        //Assert.Contains(postResponseBody, listOfAllPlatforms);
        var first = listOfAllPlatforms.First();
        Assert.Equal(postResponseBody.Created, first.Created, TimeSpan.FromMilliseconds(10));
    }
}
