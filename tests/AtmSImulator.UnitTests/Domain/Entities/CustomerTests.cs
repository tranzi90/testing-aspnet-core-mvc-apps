using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Domain.Entities
{
    [TestFixture]
    public class CustomerTests : BaseTest
    {
        #region Creation
        [Test]
        public void Customer_with_invalid_arguments_throw_exceptions()
        {
            // Arrange
            Action createWithNullCustomerName = () => Customer.Create(
                null,
                decimal.Zero,
                Guid.Empty);
            Action createWithNegativeBalance = () => Customer.Create(
                FakeCustomerNames.Valid.Generate(),
                decimal.MinusOne,
                Guid.Empty);
            Action createWithDefaultAccountId = () => Customer.Create(
                FakeCustomerNames.Valid.Generate(),
                decimal.Zero,
                Guid.Empty);

            // Assert
            Assert.Multiple(() =>
            {
                createWithNullCustomerName.Should().Throw<ArgumentNullException>();
                createWithNegativeBalance.Should().Throw<ArgumentException>();
                createWithDefaultAccountId.Should().Throw<ArgumentException>();
            });
        }

        [Test]
        public void Customer_with_valid_arguments_is_created()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var cash = decimal.One;
            var accountId = Faker.Random.Guid();

            Func<Customer> createCustomer = () => Customer.Create(customerName, cash, accountId);

            // Assert
            createCustomer.Should().NotThrow();

            Assert.Multiple(() =>
            {
                var customer = createCustomer();

                customer.Name.Should().Be(customerName);
                customer.Cash.Should().Be(cash);
                customer.AccountId.Should().Be(accountId);
            });
        }

        [Test]
        public void Customer_with_negative_cash_has_Failure_result()
        {
            // Arrange
            var customer = Customer.TryCreate(
                FakeCustomerNames.Valid.Generate(),
                decimal.MinusOne,
                Faker.Random.Guid());

            // Assert
            customer.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Customer_with_negative_cash_is_invalid()
        {
            // Arrange
            var customer = Customer.Validate(
                decimal.MinusOne,
                Guid.Empty);

            // Assert
            customer.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Customer_with_default_account_id_has_Failure_result()
        {
            // Arrange
            var customer = Customer.TryCreate(
                FakeCustomerNames.Valid.Generate(),
                decimal.One,
                Guid.Empty);

            // Assert
            customer.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Customer_with_default_account_id_is_invalid()
        {
            // Arrange
            var customer = Customer.Validate(
                decimal.One,
                Guid.Empty);

            // Assert
            customer.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Customer_with_valid_arguments_has_Success_result()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var cash = decimal.One;
            var accountId = Faker.Random.Guid();

            Func<Result<Customer>> createCustomer = () => Customer.TryCreate(
                customerName,
                cash,
                accountId);

            // Assert
            createCustomer.Should().NotThrow();

            var maybeCustomer = createCustomer();

            maybeCustomer.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var customer = maybeCustomer.Value;

                customer.Name.Should().Be(customerName);
                customer.Cash.Should().Be(cash);
                customer.AccountId.Should().Be(accountId);
            });
        }
        #endregion

        #region Transfer
        [Test, Sequential]
        public void Can_withdraw_when_enough_money(
            [Values(5, 10)] decimal cash,
            [Values(5, 9)] decimal amount,
            [Values(0, 1)] decimal expectedCash)
        {
            // Arrange
            var customer = Customer.Create(
                FakeCustomerNames.Valid.Generate(),
                cash,
                Faker.Random.Guid());

            // Act
            var withdrawResult = customer.Withdraw(amount);

            // Assert
            Assert.Multiple(() =>
            {
                withdrawResult.IsSuccess.Should().BeTrue();
                customer.Cash.Should().Be(expectedCash);
            });
        }

        [Test, Sequential]
        public void Can_not_withdraw_when_not_enough_money(
            [Values(5, 10)] decimal cash,
            [Values(6, 11)] decimal amount)
        {
            // Arrange
            var customer = Customer.Create(
                FakeCustomerNames.Valid.Generate(),
                cash,
                Faker.Random.Guid());

            // Act
            var withdrawResult = customer.Withdraw(amount);

            // Assert
            Assert.Multiple(() =>
            {
                withdrawResult.IsFailure.Should().BeTrue();
                customer.Cash.Should().Be(cash);
            });
        }

        [Test]
        public void Deposit_is_always_successful()
        {
            // Arrange
            var customer = Customer.Create(
                FakeCustomerNames.Valid.Generate(),
                decimal.Zero,
                Faker.Random.Guid());

            var amount = Faker.Random.Decimal();

            // Act
            customer.Deposit(amount);

            // Assert
            customer.Cash.Should().Be(amount);
        }
        #endregion
    }
}
