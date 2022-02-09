using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Application
{
    [TestFixture]
    public class FinancialTransferSystemTests : BaseTest
    {
        [Test]
        public void Customer_can_deposit_to_Atm()
        {
            // Arrange
            var accountId = Faker.Random.Guid();
            var customerName = FakeCustomerNames.Valid.Generate();
            var customer = Customer.Create(customerName, decimal.One, accountId);
            var paymentCard = FakePaymentCards.ValidForCustomer(customerName).Generate();
            var account = Account.Create(
                accountId,
                customerName,
                decimal.Zero,
                new[]
                {
                    paymentCard,
                });
            var atmId = Faker.Random.Guid();
            var atm = Atm.Create(
                atmId,
                decimal.Zero);

            var customerRepository = Substitute.For<ICustomerRepository>();
            var accountRepository = Substitute.For<IAccountRepository>();
            var atmRepository = Substitute.For<IAtmRepository>();

            customerRepository.Get(customerName).Returns(customer);
            accountRepository.Get(paymentCard.Number).Returns(account);
            atmRepository.Get(atmId).Returns(atm);

            customerRepository.Update(customer).Returns(Result.Success());
            accountRepository.Update(account).Returns(Result.Success());
            atmRepository.Update(atm).Returns(Result.Success());

            var financialTransferSystemService = new FinancialTransferSystemService(
                TransferService,
                customerRepository,
                accountRepository,
                atmRepository);

            // Act
            var depositResult = financialTransferSystemService.DepositToAtm(paymentCard.Number, atm.Id, decimal.One);

            // Result
            depositResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                customer.Cash.Should().Be(decimal.Zero);
                account.Balance.Should().Be(decimal.One);
                atm.Balance.Should().Be(decimal.One);

                customerRepository.Received(1).Get(customerName);
                accountRepository.Received(1).Get(paymentCard.Number);
                atmRepository.Received(1).Get(atmId);

                customerRepository.Received(1).Update(customer);
                accountRepository.Received(1).Update(account);
                atmRepository.Received(1).Update(atm);
            });
        }

        [Test]
        public void Customer_can_withdraw_from_Atm()
        {
            // Arrange
            var accountId = Faker.Random.Guid();
            var customerName = FakeCustomerNames.Valid.Generate();
            var customer = Customer.Create(customerName, decimal.Zero, accountId);
            var paymentCard = FakePaymentCards.ValidForCustomer(customerName).Generate();
            var account = Account.Create(
                accountId,
                customerName,
                decimal.One,
                new[]
                {
                    paymentCard,
                });
            var atmId = Faker.Random.Guid();
            var atm = Atm.Create(
                atmId,
                decimal.One);

            var customerRepository = Substitute.For<ICustomerRepository>();
            var accountRepository = Substitute.For<IAccountRepository>();
            var atmRepository = Substitute.For<IAtmRepository>();

            customerRepository.Get(customerName).Returns(customer);
            accountRepository.Get(paymentCard.Number).Returns(account);
            atmRepository.Get(atmId).Returns(atm);

            customerRepository.Update(customer).Returns(Result.Success());
            accountRepository.Update(account).Returns(Result.Success());
            atmRepository.Update(atm).Returns(Result.Success());

            var financialTransferSystemService = new FinancialTransferSystemService(
                TransferService,
                customerRepository,
                accountRepository,
                atmRepository);

            // Act
            var withdrawResult = financialTransferSystemService.WithdrawFromAtm(paymentCard.Number, atm.Id, decimal.One);

            // Result
            withdrawResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                customer.Cash.Should().Be(decimal.One);
                account.Balance.Should().Be(decimal.Zero);
                atm.Balance.Should().Be(decimal.Zero);

                customerRepository.Received(1).Get(customerName);
                accountRepository.Received(1).Get(paymentCard.Number);
                atmRepository.Received(1).Get(atmId);

                customerRepository.Received(1).Update(customer);
                accountRepository.Received(1).Update(account);
                atmRepository.Received(1).Update(atm);
            });
        }

        [Test]
        public void Customer_can_transfer_to_another_customer()
        {
            // Arrange
            var senderAccountId = Faker.Random.Guid();
            var senderCustomerName = FakeCustomerNames.Valid.Generate();
            var senderPaymentCard = FakePaymentCards.ValidForCustomer(senderCustomerName).Generate();
            var sender = Account.Create(
                senderAccountId,
                senderCustomerName,
                decimal.One,
                new[]
                {
                    senderPaymentCard,
                });

            var recipientAccountId = Faker.Random.Guid();
            var recipientCustomerName = FakeCustomerNames.Valid.Generate();
            var recipientPaymentCard = FakePaymentCards.ValidForCustomer(recipientCustomerName).Generate();
            var recipient = Account.Create(
                recipientAccountId,
                recipientCustomerName,
                decimal.Zero,
                new[]
                {
                    recipientPaymentCard,
                });

            var customerRepository = Substitute.For<ICustomerRepository>();
            var accountRepository = Substitute.For<IAccountRepository>();
            var atmRepository = Substitute.For<IAtmRepository>();

            accountRepository.Get(senderPaymentCard.Number).Returns(sender);
            accountRepository.Get(recipientPaymentCard.Number).Returns(recipient);

            accountRepository.Update(sender).Returns(Result.Success());
            accountRepository.Update(recipient).Returns(Result.Success());

            var financialTransferSystemService = new FinancialTransferSystemService(
                TransferService,
                customerRepository,
                accountRepository,
                atmRepository);

            // Act
            var transferResult = financialTransferSystemService.TransferToAnotherCustomer(
                senderPaymentCard.Number,
                recipientPaymentCard.Number,
                decimal.One);

            // Result
            transferResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                sender.Balance.Should().Be(decimal.Zero);
                recipient.Balance.Should().Be(decimal.One);

                accountRepository.Received(1).Get(senderPaymentCard.Number);
                accountRepository.Received(1).Get(recipientPaymentCard.Number);

                accountRepository.Received(1).Update(sender);
                accountRepository.Received(1).Update(recipient);
            });
        }
    }
}
