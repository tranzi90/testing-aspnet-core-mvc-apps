using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Domain.Entities
{
    [TestFixture]
    public class PaymentCardNumberTests : BaseTest
    {
        #region Creation
        [Test]
        public void Payment_card_number_with_invalid_arguments_throw_exceptions()
        {
            // Arrange
            Action createWithFirstGroupNegative = () => PaymentCardNumber.Create(
                short.MinValue,
                short.MaxValue,
                short.MaxValue,
                short.MaxValue);
            Action createWithSecondGroupNegative = () => PaymentCardNumber.Create(
                short.MaxValue,
                short.MinValue,
                short.MaxValue,
                short.MaxValue);
            Action createWithThirdGroupNegative = () => PaymentCardNumber.Create(
                short.MaxValue,
                short.MaxValue,
                short.MinValue,
                short.MaxValue);
            Action createWithFourthGroupNegative = () => PaymentCardNumber.Create(
                short.MaxValue,
                short.MaxValue,
                short.MaxValue,
                short.MinValue);

            // Assert
            Assert.Multiple(() =>
            {
                createWithFirstGroupNegative.Should().Throw<ArgumentException>();
                createWithSecondGroupNegative.Should().Throw<ArgumentException>();
                createWithThirdGroupNegative.Should().Throw<ArgumentException>();
                createWithFourthGroupNegative.Should().Throw<ArgumentException>();
            });
        }

        [Test]
        public void Payment_card_number_with_valid_arguments_is_created()
        {
            // Arrange
            var firstGroup = Faker.Random.Short(1);
            var secondGroup = Faker.Random.Short(1);
            var thirdGroup = Faker.Random.Short(1);
            var fourthGroup = Faker.Random.Short(1);

            Func<PaymentCardNumber> createPaymentCardNumber = () => PaymentCardNumber.Create(
                firstGroup,
                secondGroup,
                thirdGroup,
                fourthGroup);

            // Assert
            createPaymentCardNumber.Should().NotThrow();

            Assert.Multiple(() =>
            {
                var paymentCardNumber = createPaymentCardNumber();

                paymentCardNumber.FirstGroup.Should().Be(firstGroup);
                paymentCardNumber.SecondGroup.Should().Be(secondGroup);
                paymentCardNumber.ThirdGroup.Should().Be(thirdGroup);
                paymentCardNumber.FourthGroup.Should().Be(fourthGroup);
            });
        }

        [Test]
        public void Payment_card_number_with_valid_arguments_has_Success_result()
        {
            // Arrange
            var firstGroup = Faker.Random.Short(1);
            var secondGroup = Faker.Random.Short(1);
            var thirdGroup = Faker.Random.Short(1);
            var fourthGroup = Faker.Random.Short(1);

            Func<Result<PaymentCardNumber>> createPaymentCardNumber = () => PaymentCardNumber.TryCreate(
                firstGroup,
                secondGroup,
                thirdGroup,
                fourthGroup);

            // Assert
            createPaymentCardNumber.Should().NotThrow();

            var maybePaymentCardNumber = createPaymentCardNumber();

            maybePaymentCardNumber.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var paymentCardNumber = maybePaymentCardNumber.Value;

                paymentCardNumber.FirstGroup.Should().Be(firstGroup);
                paymentCardNumber.SecondGroup.Should().Be(secondGroup);
                paymentCardNumber.ThirdGroup.Should().Be(thirdGroup);
                paymentCardNumber.FourthGroup.Should().Be(fourthGroup);
            });
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("234324")]
        [TestCase("dsaf")]
        [TestCase("dsaf32fdf")]
        public void Payment_card_number_is_not_correctly_parsed(string number)
        {
            // Act
            var parsed = PaymentCardNumber.TryParse(number);

            // Assert
            parsed.IsFailure.Should().BeTrue();
        }

        [Test]
        [TestCase("1234-5678-2345-0392", 1234, 5678, 2345, 392)]
        [TestCase("0002-8643-9273-1325", 2, 8643, 9273, 1325)]
        [TestCase("7255-1238-3925-4852", 7255, 1238, 3925, 4852)]
        public void Payment_card_number_is_correctly_parsed(
            string number,
            short firstGroup,
            short secondGroup,
            short thirdGroup,
            short fourthGroup)
        {
            // Act
            var parsed = PaymentCardNumber.TryParse(number);

            // Arrange
            parsed.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var paymentCardNumber = parsed.Value;

                paymentCardNumber.FirstGroup.Should().Be(firstGroup);
                paymentCardNumber.SecondGroup.Should().Be(secondGroup);
                paymentCardNumber.ThirdGroup.Should().Be(thirdGroup);
                paymentCardNumber.FourthGroup.Should().Be(fourthGroup);

                paymentCardNumber.ToString().Should().Be(number);
            });
        }
        #endregion
    }
}
