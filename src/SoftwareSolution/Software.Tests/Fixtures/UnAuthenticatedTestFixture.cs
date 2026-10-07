

using Alba;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Software.Tests;

public class UnAuthenticatedTestFixture : IAsyncLifetime
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