

using Alba;
using Alba.Security;
using Marten;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Software.Api.Vendors;
using Testcontainers.PostgreSql;

namespace Software.Tests;

[CollectionDefinition("SoftwareSystemTests")]

public class SoftwareTestCollection : ICollectionFixture<TestFixture> { }

[CollectionDefinition("OtherSystemTests")]
public class OtherTestCollection : ICollectionFixture<TestFixture> { }

public class TestFixture : IAsyncLifetime
{

    public IAlbaHost Host { get; set; } = null!;
    private PostgreSqlContainer _pgContainer = null!;


    public async Task InitializeAsync()
    {
        _pgContainer = new PostgreSqlBuilder("postgres:18").Build();
     
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
        }, new AuthenticationStub());
    }
    public async Task DisposeAsync()
    {
        await _pgContainer.StopAsync();
        await Host.StopAsync();
    }
    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        // Add any additional test-specific services here
       
    }
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Add any additional services here
    }
}



[CollectionDefinition("UnitIntegrationTests")]
public class UnitIntegrationCollection : ICollectionFixture<UnitIntegrationFixture> { }
public class UnitIntegrationFixture : TestFixture {

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        //var fakeNameValidator = Substitute.For<INormalizeVendorNames>();
        //fakeNameValidator.NormalizeName(Arg.Any<string>()).Returns(x => Guid.NewGuid().ToString());
        //services.AddScoped(_ => fakeNameValidator);
    }
}