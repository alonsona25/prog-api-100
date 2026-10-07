

namespace Games.Tests;

public class BadApiTest
{
    [Fact(Skip ="Do not do it this way unless you like pain")]
    public async Task GetGetStatus()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5022");

        var responseMessage = await client.GetAsync("/status");

        Assert.Equal(System.Net.HttpStatusCode.OK, responseMessage.StatusCode);
    }
}
