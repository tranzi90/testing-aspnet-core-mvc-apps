using System;
using AtmSimulator.Web.Models.Domain;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Domain.Services
{
    [TestFixture]
    public class PaymentCardGeneratorTests : BaseTest
    {
        [Test, Sequential]
        public void Generate_correct_payment_card(
            [Range(2020, 2050)] int year,
            [Range(2021, 2051)] int nextYear)
        {
            // Arrange
            var randomGenerator = Substitute.For<IRandomGenerator>();
            randomGenerator.NextPositiveShort().Returns(x => Faker.Random.Short(1));

            var paymentCardGenerator = new PaymentCardGenerator(randomGenerator);

            var customerName = FakeCustomerNames.Valid.Generate();
            var utcNow = new DateTimeOffset(year, 1, 2, 3, 4, 5, TimeSpan.Zero);

            // Act
            var paymentCard = paymentCardGenerator.GenerateNewCard(customerName, utcNow);

            // Assert
            Assert.Multiple(() =>
            {
                paymentCard.CardholderName.Should().Be(customerName);
                paymentCard.ExpirationDate.Year.Should().Be(nextYear);
            });
        }
    }
}
