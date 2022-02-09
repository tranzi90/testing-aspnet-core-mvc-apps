using System;
using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Application
{
    [TestFixture]
    public class FinancialInformationServiceTests : BaseTest
    {
        [Test]
        public void Customer_can_check_his_cash()
        {
            // Arrange
            var customerCash = decimal.One;
            var customerName = FakeCustomerNames.Valid.Generate();
            var customer = Customer.Create(
                customerName,
                customerCash,
                Faker.Random.Guid());

            var customerRepository = Substitute.For<ICustomerRepository>();
            customerRepository.Get(customerName).Returns(customer);

            var financialInformationService = new FinancialInformationService(
                customerRepository,
                null,
                null);

            // Act
            var cashResult = financialInformationService.CheckCustomerCash(customerName);

            // Assert
            cashResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var cash = cashResult.Value;

                cash.Should().Be(customerCash);
                customerRepository.Received(1).Get(customerName);
            });
        }

        [Test]
        public void Account_balance_is_checked()
        {
            // Arrange
            var accountBalance = decimal.One;
            var paymentCardNumber = FakePaymentCardNumbers.Valid.Generate();
            var account = Account.Create(
                Faker.Random.Guid(),
                FakeCustomerNames.Valid.Generate(),
                accountBalance,
                Array.Empty<PaymentCard>());

            var accountRepository = Substitute.For<IAccountRepository>();
            accountRepository.Get(paymentCardNumber).Returns(account);

            var financialInformationService = new FinancialInformationService(
                null,
                accountRepository,
                null);

            // Act
            var balanceResult = financialInformationService.CheckAccountBalance(paymentCardNumber);

            // Assert
            balanceResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var balance = balanceResult.Value;

                balance.Should().Be(accountBalance);
                accountRepository.Received(1).Get(paymentCardNumber);
            });
        }

        [Test]
        public void Atm_balance_is_checked()
        {
            // Arrange
            var atmId = Faker.Random.Guid();
            var atmbalance = decimal.One;
            var atm = Atm.Create(
                atmId,
                atmbalance);

            var atmRepository = Substitute.For<IAtmRepository>();
            atmRepository.Get(atmId).Returns(atm);

            var financialInformationService = new FinancialInformationService(
                null,
                null,
                atmRepository);

            // Act
            var balanceResult = financialInformationService.CheckAtmBalance(atmId);

            // Assert
            balanceResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var balance = balanceResult.Value;

                balance.Should().Be(atmbalance);
                atmRepository.Received(1).Get(atmId);
            });
        }
    }
}
