using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Domain.Entities
{
    [TestFixture]
    public class AtmTests : BaseTest
    {
        #region Creation
        [Test]
        public void Atm_with_invalid_arguments_throw_exceptions()
        {
            // Arrange
            Action createWithDefaultId = () => Atm.Create(Guid.Empty, decimal.Zero);
            Action createWithNegativeBalance = () => Atm.Create(Faker.Random.Guid(), decimal.MinusOne);

            // Assert
            Assert.Multiple(() =>
            {
                createWithDefaultId.Should().Throw<ArgumentException>();
                createWithNegativeBalance.Should().Throw<ArgumentException>();
            });
        }

        [Test]
        public void Atm_with_valid_arguments_is_created()
        {
            // Arrange
            var id = Faker.Random.Guid();
            var balance = decimal.One;

            Func<Atm> createAtm = () => Atm.Create(id, balance);

            // Assert
            createAtm.Should().NotThrow();

            Assert.Multiple(() =>
            {
                var atm = createAtm();

                atm.Id.Should().Be(id);
                atm.Balance.Should().Be(balance);
            });
        }

        [Test]
        public void Atm_with_default_id_has_Failure_result()
        {
            // Arrange
            var atm = Atm.TryCreate(
                Guid.Empty,
                decimal.Zero);

            // Assert
            atm.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Atm_with_default_id_is_invalid()
        {
            // Arrange
            var atm = Atm.Validate(
                Guid.Empty,
                decimal.Zero);

            // Assert
            atm.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Atm_with_negative_balance_has_Failure_result()
        {
            // Arrange
            var atm = Atm.TryCreate(
                Faker.Random.Guid(),
                decimal.MinusOne);

            // Assert
            atm.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Atm_with_negative_balance_is_invalid()
        {
            // Arrange
            var atm = Atm.Validate(
                Faker.Random.Guid(),
                decimal.MinusOne);

            // Assert
            atm.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Atm_with_valid_arguments_has_Success_result()
        {
            // Arrange
            var id = Faker.Random.Guid();
            var balance = decimal.One;

            Func<Result<Atm>> createAtm = () => Atm.TryCreate(
                id,
                balance);

            // Assert
            createAtm.Should().NotThrow();

            var maybeAtm = createAtm();

            maybeAtm.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var atm = maybeAtm.Value;

                atm.Id.Should().Be(id);
                atm.Balance.Should().Be(balance);
            });
        }
        #endregion

        #region Transfer
        [Test, Sequential]
        public void Can_withdraw_when_enough_money(
            [Values(5, 10)] decimal balance,
            [Values(5, 9)] decimal amount,
            [Values(0, 1)] decimal expectedBalance)
        {
            // Arrange
            var atm = Atm.Create(
                Faker.Random.Guid(),
                balance);

            // Act
            var withdrawResult = atm.Withdraw(amount);

            // Assert
            Assert.Multiple(() =>
            {
                withdrawResult.IsSuccess.Should().BeTrue();
                atm.Balance.Should().Be(expectedBalance);
            });
        }

        [Test, Sequential]
        public void Can_not_withdraw_when_not_enough_money(
            [Values(5, 10)] decimal balance,
            [Values(6, 11)] decimal amount)
        {
            // Arrange
            var atm = Atm.Create(
                Faker.Random.Guid(),
                balance);

            // Act
            var withdrawResult = atm.Withdraw(amount);

            // Assert
            Assert.Multiple(() =>
            {
                withdrawResult.IsFailure.Should().BeTrue();
                atm.Balance.Should().Be(balance);
            });
        }

        [Test]
        public void Deposit_is_always_successful()
        {
            // Arrange
            var atm = Atm.Create(
                Faker.Random.Guid(),
                decimal.Zero);

            var amount = Faker.Random.Decimal();

            // Act
            atm.Deposit(amount);

            // Assert
            atm.Balance.Should().Be(amount);
        }
        #endregion
    }
}
