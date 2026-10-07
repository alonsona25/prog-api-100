

using Alba;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.PostgreSql;

namespace Games.Tests;

[CollectionDefinition("GamesSystemTests")]
public class GamesTestCollection : ICollectionFixture<TestFixture> { }
public class TestFixture : IAsyncLifetime
{

    public IAlbaHost Host { get; set; } = null!;
    private PostgreSqlContainer _pgContainer = null;


    public async Task InitializeAsync()
    {
        _pgContainer = new PostgreSqlBuilder("postgres:17").Build();
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(1969, 4, 20, 23, 59, 59, TimeSpan.FromHours(-04)));
        
        await _pgContainer.StartAsync();
        Host = await AlbaHost.For<Program>(options =>
        {
            options.UseSetting("ConnectionStrings:games-db", _pgContainer.GetConnectionString());
            options.ConfigureServices(svc =>
            {

            });
            options.ConfigureTestServices(svc =>
            {
                svc.AddSingleton<TimeProvider>(fakeTimeProvider);
            });
        });
    }
    public async Task DisposeAsync()
    {
        await _pgContainer.StopAsync();
        await Host.StopAsync();
    }
}
