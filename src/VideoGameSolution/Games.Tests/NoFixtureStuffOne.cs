

namespace Games.Tests;


public class SharedFixture : IAsyncLifetime
{
    public Guid TheId { get; set; } = Guid.NewGuid();
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
    public async Task InitializeAsync()
    {
        var t = 0;
    }

}


public class AnotherUsingTheFixture(SharedFixture fixture) : IClassFixture<SharedFixture>
{
    [Fact]
    public void Test1()
    {
        var id = fixture.TheId;
    }
    [Fact]
    public void Test2()
    {
        var id = fixture.TheId;
    }
})
public class NoFixtureStuffOne(SharedFixture fixture): IClassFixture<SharedFixture>
{
  
 



    [Fact]
    public void Test1()
    {
    
    }
    [Fact]
    public void Test2()
    {
        
    }
}
