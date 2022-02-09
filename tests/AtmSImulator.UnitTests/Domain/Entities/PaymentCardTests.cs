using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Domain.Entities
{
    [TestFixture]
    public class PaymentCardTests : BaseTest
    {
        #region Creation
        [Test]
        public void Payment_card_with_invalid_arguments_throw_exceptions()
        {
            // Arrange
            Action createWithNullNumber = () => PaymentCard.Create(
                null,
                null,
                DateTimeOffset.MinValue,
                short.MinValue);
            Action createWithNullCustomerName = () => PaymentCard.Create(
                FakePaymentCardNumbers.Valid.Generate(),
                null,
                DateTimeOffset.MinValue,
                short.MinValue);
            Action createWithDefaultExpirationDate = () => PaymentCard.Create(
                FakePaymentCardNumbers.Valid.Generate(),
                FakeCustomerNames.Valid.Generate(),
                DateTimeOffset.MinValue,
                short.MinValue);
            Action createWithNegativeSecurityCode = () => PaymentCard.Create(
                FakePaymentCardNumbers.Valid.Generate(),
                FakeCustomerNames.Valid.Generate(),
                DateTimeOffset.UtcNow,
                short.MinValue);

            // Assert
            Assert.Multiple(() =>
            {
                createWithNullNumber.Should().Throw<ArgumentNullException>();
                createWithNullCustomerName.Should().Throw<ArgumentNullException>();
                createWithDefaultExpirationDate.Should().Throw<ArgumentException>();
                createWithNegativeSecurityCode.Should().Throw<ArgumentException>();
            });
        }

        [Test]
        public void Payment_card_with_valid_arguments_is_created()
        {
            // Arrange
            var number = FakePaymentCardNumbers.Valid.Generate();
            var cardholderName = FakeCustomerNames.Valid.Generate();
            var expirationDate = Faker.Date.FutureOffset();
            var securityCode = Faker.Random.Short(1);

            Func<PaymentCard> createPaymentCard = () => PaymentCard.Create(
                number,
                cardholderName,
                expirationDate,
                securityCode);

            // Assert
            createPaymentCard.Should().NotThrow();

            Assert.Multiple(() =>
            {
                var paymentCard = createPaymentCard();

                paymentCard.Number.Should().Be(number);
                paymentCard.CardholderName.Should().Be(cardholderName);
                paymentCard.ExpirationDate.Should().Be(expirationDate);
                paymentCard.SecurityCode.Should().Be(securityCode);
            });
        }

        [Test]
        public void Payment_card_with_expiration_date_less_than_now_has_Failure_result()
        {
            // Arrange
            var number = FakePaymentCardNumbers.Valid.Generate();
            var cardholderName = FakeCustomerNames.Valid.Generate();
            var expirationDate = Faker.Date.FutureOffset();
            var securityCode = Faker.Random.Short(1);

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(expirationDate.AddDays(1));

            var paymentCard = PaymentCard.TryCreate(
                number,
                cardholderName,
                dateTimeProvider,
                expirationDate,
                securityCode);

            // Assert
            paymentCard.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Payment_card_with_expiration_date_less_than_now_is_invalid()
        {
            // Arrange
            var expirationDate = Faker.Date.FutureOffset();
            var securityCode = Faker.Random.Short(1);

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(expirationDate.AddDays(1));

            var paymentCard = PaymentCard.Validate(
                dateTimeProvider,
                expirationDate,
                securityCode);

            // Assert
            paymentCard.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Payment_card_with_negative_security_code_has_Failure_result()
        {
            // Arrange
            var number = FakePaymentCardNumbers.Valid.Generate();
            var cardholderName = FakeCustomerNames.Valid.Generate();
            var expirationDate = Faker.Date.FutureOffset();

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(expirationDate.AddDays(-1));

            var paymentCard = PaymentCard.TryCreate(
                number,
                cardholderName,
                dateTimeProvider,
                expirationDate,
                short.MinValue);

            // Assert
            paymentCard.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Payment_card_with_negative_security_code_is_invalid()
        {
            // Arrange
            var expirationDate = Faker.Date.FutureOffset();

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(expirationDate.AddDays(-1));

            var paymentCard = PaymentCard.Validate(
                dateTimeProvider,
                expirationDate,
                short.MinValue);

            // Assert
            paymentCard.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Payment_card_with_valid_arguments_has_Success_result()
        {
            // Arrange
            var number = FakePaymentCardNumbers.Valid.Generate();
            var cardholderName = FakeCustomerNames.Valid.Generate();
            var expirationDate = Faker.Date.FutureOffset();
            var securityCode = Faker.Random.Short(1);

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(expirationDate.AddDays(-1));

            Func<Result<PaymentCard>> createPaymentCard = () => PaymentCard.TryCreate(
                number,
                cardholderName,
                dateTimeProvider,
                expirationDate,
                securityCode);

            // Assert
            createPaymentCard.Should().NotThrow();

            var maybePaymentCard = createPaymentCard();

            maybePaymentCard.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var paymentCard = maybePaymentCard.Value;

                paymentCard.Number.Should().Be(number);
                paymentCard.CardholderName.Should().Be(cardholderName);
                paymentCard.ExpirationDate.Should().Be(expirationDate);
                paymentCard.SecurityCode.Should().Be(securityCode);
            });
        }
        #endregion
    }
}
