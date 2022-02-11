using System;
using AtmSimulator.IntegrationTests.Fakes;
using AtmSimulator.Web.Models.Domain;
using Bogus;
using NSubstitute;
using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]

namespace AtmSimulator.IntegrationTests
{
    public abstract class BaseTest
    {
        protected Faker Faker { get; private set; }

        protected CustomerNameFakeData FakeCustomerNames { get; private set; }

        protected PaymentCardNumberFakeData FakePaymentCardNumbers { get; private set; }

        protected PaymentCardFakeData FakePaymentCards { get; private set; }

        protected AtmFakeData FakeAtms { get; private set; }

        protected CustomerFakeData FakeCustomers { get; private set; }

        protected IRandomGenerator RandomGenerator { get; private set; }

        protected IDateTimeProvider DateTimeProvider { get; private set; }

        protected PaymentCardGenerator PaymentCardGenerator { get; private set; }

        protected TransferService TransferService { get; private set; }

        protected FakeAtmRepository FakeAtmRepository { get; private set; }

        protected DateTimeOffset DefaultUtcNow = new DateTimeOffset(2000, 1, 2, 3, 4, 5, TimeSpan.Zero);

        [SetUp]
        public void SetupBeforeEachTest()
        {
            const int seed = 1234;

            Randomizer.Seed = new Random(seed);

            Faker = new Faker("uk");

            FakeCustomerNames = new CustomerNameFakeData(seed);
            FakePaymentCardNumbers = new PaymentCardNumberFakeData(seed);
            FakePaymentCards = new PaymentCardFakeData(seed, FakePaymentCardNumbers);
            FakeAtms = new AtmFakeData(seed);
            FakeCustomers = new CustomerFakeData(seed, FakeCustomerNames);
            RandomGenerator = Substitute.For<IRandomGenerator>();
            RandomGenerator.NextPositiveShort().Returns(x => Faker.Random.Short(1));
            RandomGenerator.NewGuid().Returns(x => Guid.NewGuid());
            DateTimeProvider = Substitute.For<IDateTimeProvider>();
            DateTimeProvider.UtcNow.Returns(x => DefaultUtcNow);
            PaymentCardGenerator = new PaymentCardGenerator(RandomGenerator);
            TransferService = new TransferService();
            FakeAtmRepository = new FakeAtmRepository(FakeAtms);
        }
    }
}
