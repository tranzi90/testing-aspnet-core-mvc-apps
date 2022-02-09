using System;
using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Application
{
    [TestFixture]
    public class FinancialInstitutionServiceTests : BaseTest
    {
        [Test]
        public void Customer_is_registered()
        {
            // Arrange
            var accountId = Faker.Random.Guid();
            var customerName = FakeCustomerNames.Valid.Generate();
            var customerCash = decimal.One;
            var customer = Customer.Create(customerName, customerCash, accountId);
            var accountBalance = decimal.One;
            var account = Account.Create(
                accountId,
                customerName,
                accountBalance,
                Array.Empty<PaymentCard>());

            var customerRepository = Substitute.For<ICustomerRepository>();
            var accountRepository = Substitute.For<IAccountRepository>();
            var atmRepository = Substitute.For<IAtmRepository>();

            customerRepository.Get(customerName).Returns(Maybe<Customer>.None);
            customerRepository.Register(customer).Returns(Result.Success());
            accountRepository.Register(account).Returns(Result.Success());

            var randomGenerator = Substitute.For<IRandomGenerator>();
            randomGenerator.NewGuid().Returns(accountId);

            var financialInstitutionService = new FinancialInstitutionService(
                PaymentCardGenerator,
                customerRepository,
                accountRepository,
                atmRepository,
                randomGenerator,
                DateTimeProvider);

            // Act
            var registerCustomerResult = financialInstitutionService.RegisterCustomer(customerName, customerCash);

            // Assert
            registerCustomerResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var registeredCustomer = registerCustomerResult.Value;

                registeredCustomer.Should().Be(customer);

                customerRepository.Received(1).Get(customerName);
                customerRepository.Received(1).Register(customer);
                accountRepository.Received(1).Register(account);
            });
        }

        [Test]
        public void Atm_is_registered()
        {
            // Arrange
            var atmId = Faker.Random.Guid();
            var atmBalance = decimal.One;
            var atm = Atm.Create(atmId, atmBalance);

            var customerRepository = Substitute.For<ICustomerRepository>();
            var accountRepository = Substitute.For<IAccountRepository>();
            var atmRepository = Substitute.For<IAtmRepository>();

            atmRepository.Register(atm).Returns(Result.Success());

            var randomGenerator = Substitute.For<IRandomGenerator>();
            randomGenerator.NewGuid().Returns(atmId);

            var financialInstitutionService = new FinancialInstitutionService(
                PaymentCardGenerator,
                customerRepository,
                accountRepository,
                atmRepository,
                randomGenerator,
                DateTimeProvider);

            // Act
            var registerAtmResult = financialInstitutionService.RegisterAtm(atmBalance);

            // Assert
            registerAtmResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var registeredAtm = registerAtmResult.Value;

                registeredAtm.Should().Be(atm);

                atmRepository.Received(1).Register(atm);
            });
        }

        [Test]
        public void New_payment_card_is_issued()
        {
            // Arrange
            var accountId = Faker.Random.Guid();
            var customerName = FakeCustomerNames.Valid.Generate();
            var accountBalance = decimal.One;
            var account = Account.Create(
                accountId,
                customerName,
                accountBalance,
                Array.Empty<PaymentCard>());

            var customerRepository = Substitute.For<ICustomerRepository>();
            var accountRepository = Substitute.For<IAccountRepository>();
            var atmRepository = Substitute.For<IAtmRepository>();

            accountRepository.Get(customerName).Returns(account);
            accountRepository.Update(account).Returns(Result.Success());

            var financialInstitutionService = new FinancialInstitutionService(
                PaymentCardGenerator,
                customerRepository,
                accountRepository,
                atmRepository,
                RandomGenerator,
                DateTimeProvider);

            // Act
            var newPaymentCardResult = financialInstitutionService.IssueNewPaymentCard(customerName);

            // Assert
            newPaymentCardResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var newPaymentCard = newPaymentCardResult.Value;

                account.PaymentCards.Should().Contain(newPaymentCard);

                accountRepository.Received(1).Get(customerName);
                accountRepository.Received(1).Update(account);
            });
        }

        [Test]
        public void Payment_card_is_deleted()
        {
            // Arrange
            var accountId = Faker.Random.Guid();
            var customerName = FakeCustomerNames.Valid.Generate();
            var accountBalance = decimal.One;
            var paymentCard = FakePaymentCards.ValidForCustomer(customerName).Generate();
            var account = Account.Create(
                accountId,
                customerName,
                accountBalance,
                new[] 
                {
                    paymentCard,
                });

            var customerRepository = Substitute.For<ICustomerRepository>();
            var accountRepository = Substitute.For<IAccountRepository>();
            var atmRepository = Substitute.For<IAtmRepository>();

            accountRepository.Get(paymentCard.Number).Returns(account);
            accountRepository.Update(account).Returns(Result.Success());

            var financialInstitutionService = new FinancialInstitutionService(
                PaymentCardGenerator,
                customerRepository,
                accountRepository,
                atmRepository,
                RandomGenerator,
                DateTimeProvider);

            // Act
            var paymentCardDeletionResult = financialInstitutionService.DeletePaymentCard(paymentCard.Number);

            // Assert
            paymentCardDeletionResult.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                account.PaymentCards.Should().NotContain(paymentCard);

                accountRepository.Received(1).Get(paymentCard.Number);
                accountRepository.Received(1).Update(account);
            });
        }
    }
}
