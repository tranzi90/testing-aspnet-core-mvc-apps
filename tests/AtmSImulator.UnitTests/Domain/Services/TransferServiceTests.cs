using System;
using AtmSimulator.Web.Models.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Domain.Services
{
    [TestFixture]
    public class TransferServiceTests : BaseTest
    {
        private TransferService Sut;

        [SetUp]
        public void SetupTransferService()
        {
            Sut = new TransferService();
        }

        [Test]
        [TestCase(100, 0, 75, 25, 75, 75)]
        [TestCase(1000, 200, 1000, 0, 1000, 1200)]
        [TestCase(10000, 2000, 10000, 0, 10000, 12000)]
        public void Customer_can_deposit_to_Atm(
            decimal customerCash,
            decimal atmBalance,
            decimal amount,
            decimal expectedCustomerCash,
            decimal expectedAccountBalance,
            decimal expectedAtmBalance)
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var account = Account.CreateForCustomer(
                Faker.Random.Guid(),
                customerName);
            var customer = Customer.Create(
                customerName,
                customerCash,
                account.Id);
            var atm = Atm.Create(
                Faker.Random.Guid(),
                atmBalance);

            // Act
            var depositResult = Sut.DepositToAtm(customer, account, atm, amount);

            // Assert
            depositResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                customer.Cash.Should().Be(expectedCustomerCash);
                account.Balance.Should().Be(expectedAccountBalance);
                atm.Balance.Should().Be(expectedAtmBalance);
            });
        }

        [Test]
        [TestCase(0, 50, 35, 15, 15, 35, 20)]
        [TestCase(500, 50, 35, 15, 515, 35, 20)]
        [TestCase(0, 50, 35, 35, 35, 15, 0)]
        public void Customer_can_withdraw_from_Atm(
            decimal customerCash,
            decimal accountBalance,
            decimal atmBalance,
            decimal amount,
            decimal expectedCustomerCash,
            decimal expectedAccountBalance,
            decimal expectedAtmBalance)
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                accountBalance,
                Array.Empty<PaymentCard>());
            var customer = Customer.Create(
                customerName,
                customerCash,
                account.Id);
            var atm = Atm.Create(
                Faker.Random.Guid(),
                atmBalance);

            // Act
            var depositResult = Sut.WithdrawFromAtm(customer, account, atm, amount);

            // Assert
            depositResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                customer.Cash.Should().Be(expectedCustomerCash);
                account.Balance.Should().Be(expectedAccountBalance);
                atm.Balance.Should().Be(expectedAtmBalance);
            });
        }

        [Test]
        [TestCase(100, 50, 10, 90, 60)]
        [TestCase(50, 100, 10, 40, 110)]
        [TestCase(50, 0, 10, 40, 10)]
        public void Customer_can_transfer_to_another_customer(
            decimal senderBalance,
            decimal recipientBalance,
            decimal amount,
            decimal expectedSenderBalance,
            decimal expectedRecipientBalance)
        {
            // Arrange
            var sender = Account.Create(
                Faker.Random.Guid(),
                FakeCustomerNames.Valid.Generate(),
                senderBalance,
                Array.Empty<PaymentCard>());

            var recipient = Account.Create(
                Faker.Random.Guid(),
                FakeCustomerNames.Valid.Generate(),
                recipientBalance,
                Array.Empty<PaymentCard>());

            // Act
            var transferResult = Sut.TransferToAnotherAccount(sender, recipient, amount);

            // Assert
            transferResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                sender.Balance.Should().Be(expectedSenderBalance);
                recipient.Balance.Should().Be(expectedRecipientBalance);
            });
        }
    }
}
