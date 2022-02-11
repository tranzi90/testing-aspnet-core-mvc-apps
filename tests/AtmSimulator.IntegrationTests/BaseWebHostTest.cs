using System.Threading.Tasks;
using AtmSimulator.IntegrationTests.Fakes;
using AtmSimulator.Web;
using AtmSimulator.Web.Models.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using WebMotions.Fake.Authentication.JwtBearer;

namespace AtmSimulator.IntegrationTests
{
    public abstract class BaseWebHostTest : BaseTest
    {
        protected TestServer TestServer { get; private set; }

        private IHost _host;

        [SetUp]
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
                        services.AddTransient(s => RandomGenerator);
                        services.AddTransient(s => DateTimeProvider);

                        services.AddTransient<IAccountRepository, FakeAccountRepository>();
                        services.AddTransient<IAtmRepository>((s) => FakeAtmRepository);
                        services.AddTransient<ICustomerRepository, FakeCustomerRepository>();
                    });
                });

            var host = await hostBuilder.StartAsync();

            _host = host;

            TestServer = _host.GetTestServer();
        }

        [TearDown]
        public async Task BaseWebHostCleanUp()
        {
            await _host.StopAsync();

            _host?.Dispose();
        }
    }
}
