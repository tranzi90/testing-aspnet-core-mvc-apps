using System;
using System.Text.Json;
using System.Threading.Tasks;
using AtmSimulator.IntegrationTests.Fakes;
using AtmSimulator.Web;
using AtmSimulator.Web.Models.Domain;
using Bogus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NUnit.Framework;
using WebMotions.Fake.Authentication.JwtBearer;

[assembly: Parallelizable(ParallelScope.Children)]
[assembly: LevelOfParallelism(4)]

namespace AtmSimulator.IntegrationTests
{
    public abstract class BaseTest
    {
        protected Faker Faker { get; private set; }

        protected CustomerNameFakeData FakeCustomerNames { get; private set; }

        protected PaymentCardNumberFakeData FakePaymentCardNumbers { get; private set; }

        protected PaymentCardFakeData FakePaymentCards { get; private set; }

        protected IRandomGenerator RandomGenerator { get; private set; }

        protected IDateTimeProvider DateTimeProvider { get; private set; }

        protected PaymentCardGenerator PaymentCardGenerator { get; private set; }

        protected TransferService TransferService { get; private set; }

        protected IHost Host { get; private set; }

        protected DateTimeOffset DefaultUtcNow = new DateTimeOffset(2000, 1, 2, 3, 4, 5, TimeSpan.Zero);

        [SetUp]
        public async Task SetupBeforeEachTest()
        {
            const int seed = 1234;

            Randomizer.Seed = new Random(seed);

            Faker = new Faker("uk");

            FakeCustomerNames = new CustomerNameFakeData(seed);
            FakePaymentCardNumbers = new PaymentCardNumberFakeData(seed);
            FakePaymentCards = new PaymentCardFakeData(seed, FakePaymentCardNumbers);
            RandomGenerator = Substitute.For<IRandomGenerator>();
            RandomGenerator.NextPositiveShort().Returns(x => Faker.Random.Short(1));
            RandomGenerator.NewGuid().Returns(x => Guid.NewGuid());
            DateTimeProvider = Substitute.For<IDateTimeProvider>();
            DateTimeProvider.UtcNow.Returns(x => DefaultUtcNow);
            PaymentCardGenerator = new PaymentCardGenerator(RandomGenerator);
            TransferService = new TransferService();

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
                        services.AddTransient<IAtmRepository, FakeAtmRepository>();
                        services.AddTransient<ICustomerRepository, FakeCustomerRepository>();
                    });
                });

            var host = await hostBuilder.StartAsync();

            Host = host;
        }

        [TearDown]
        public void Cleanup()
        {
            Host?.Dispose();
        }
    }
}
