using System.Threading.Tasks;
using AtmSimulator.FunctionalTests.Bdd.Fakes;
using AtmSimulator.Web;
using AtmSimulator.Web.Models.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using TechTalk.SpecFlow;
using WebMotions.Fake.Authentication.JwtBearer;

namespace AtmSimulator.FunctionalTests.Bdd
{
    public abstract class BaseWebHostTests : BaseTest
    {
        protected TestServer TestServer { get; private set; }

        private IHost _host;

        [BeforeScenario(Order = 1)]
        public async Task BaseWebHostSetUp()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseStartup<Startup>();
                    webHost.UseTestServer()
                        .ConfigureTestServices(collection =>
                        {
                            collection.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer();
                        });

                    webHost.ConfigureServices((context, services) =>
                    {
                        services.AddSingleton<IAccountRepository, FakeAccountRepository>();
                        services.AddSingleton<IAtmRepository, FakeAtmRepository>();
                        services.AddSingleton<ICustomerRepository, FakeCustomerRepository>();
                    });
                });

            var host = await hostBuilder.StartAsync();

            _host = host;

            TestServer = _host.GetTestServer();
        }

        [AfterScenario(Order = 1)]
        public async Task BaseWebHostCleanUp()
        {
            await _host.StopAsync();

            _host.Dispose();
        }
    }
}
