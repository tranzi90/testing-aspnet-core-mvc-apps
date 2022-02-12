using System;
using System.Linq;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Domain.Entities
{
    [TestFixture]
    public class AccountTests : BaseTest
    {
        #region Creation
        [Test]
        public void Account_with_invalid_arguments_throw_exceptions()
        {
            // Arrange
            Action createWithDefaultId = () => Account.Create(
                Guid.Empty,
                null,
                decimal.One,
                null);
            Action createWithNullCustomerName = () => Account.Create(
                Faker.Random.Guid(),
                null,
                decimal.One,
                null);
            Action createWithNegativeBalance = () => Account.Create(
                Faker.Random.Guid(),
                FakeCustomerNames.Valid.Generate(),
                decimal.MinusOne,
                null);
            Action createWithNullPaymentCards = () => Account.Create(
                Faker.Random.Guid(),
                FakeCustomerNames.Valid.Generate(),
                decimal.One,
                null);

            // Assert
            Assert.Multiple(() =>
            {
                createWithDefaultId.Should().Throw<ArgumentException>();
                createWithNullCustomerName.Should().Throw<ArgumentNullException>();
                createWithNegativeBalance.Should().Throw<ArgumentException>();
                createWithNullPaymentCards.Should().Throw<ArgumentNullException>();
            });
        }

        [Test]
        public void Account_with_valid_arguments_is_created()
        {
            // Arrange
            var id = Faker.Random.Guid();
            var customerName = FakeCustomerNames.Valid.Generate();
            var balance = decimal.One;
            var paymentCards = Array.Empty<PaymentCard>();

            Func<Account> createAccount = () => Account.Create(
                id,
                customerName,
                balance,
                paymentCards);

            // Assert
            createAccount.Should().NotThrow();

            Assert.Multiple(() =>
            {
                var account = createAccount();

                account.Id.Should().Be(id);
                account.CustomerName.Should().Be(customerName);
                account.Balance.Should().Be(balance);
                account.PaymentCards.Should().BeEquivalentTo(paymentCards);
            });
        }

        [Test]
        public void Account_with_default_id_has_Failure_result()
        {
            // Act
            var account = Account.TryCreate(
                Guid.Empty,
                FakeCustomerNames.Valid.Generate(),
                decimal.Zero);

            // Assert
            account.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Account_with_default_id_is_invalid()
        {
            // Act
            var account = Account.Validate(
                Guid.Empty,
                decimal.Zero);

            // Assert
            account.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Account_with_negative_balance_has_Failure_result()
        {
            // Act
            var account = Account.TryCreate(
                Faker.Random.Guid(),
                FakeCustomerNames.Valid.Generate(),
                decimal.MinusOne);

            // Assert
            account.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Account_with_negative_balance_is_invalid()
        {
            // Act
            var account = Account.Validate(
                Faker.Random.Guid(),
                decimal.MinusOne);

            // Assert
            account.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Account_with_valid_arguments_has_Success_result()
        {
            // Arrange
            var id = Faker.Random.Guid();
            var customerName = FakeCustomerNames.Valid.Generate();
            var balance = decimal.One;

            Func<Result<Account>> createAccount = () => Account.TryCreate(
                id,
                customerName,
                balance);

            // Assert
            createAccount.Should().NotThrow();

            var maybeAccount = createAccount();

            maybeAccount.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var account = maybeAccount.Value;

                account.Id.Should().Be(id);
                account.CustomerName.Should().Be(customerName);
                account.Balance.Should().Be(balance);
            });
        }

        [Test]
        public void Account_is_created_for_customer()
        {
            // Arrange
            var id = Faker.Random.Guid();
            var customerName = FakeCustomerNames.Valid.Generate();

            // Act
            var account = Account.CreateForCustomer(id, customerName);

            // Assert
            Assert.Multiple(() =>
            {
                account.Id.Should().Be(id);
                account.CustomerName.Should().Be(customerName);
                account.Balance.Should().Be(decimal.Zero);
                account.PaymentCards.Should().BeEmpty();
            });
        }
        #endregion

        #region Payment cards
        [Test]
        public void Get_contained_payment_card_is_Success(
            [Values(5)]int numberOfCards,
            [Values(0, 1, 2, 3, 4)]int numberOfCardToCheck)
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(customerName);
            var paymentCards = Enumerable.Range(0, numberOfCards)
                .Select(_ => paymentCardsFakeGenerator.Generate())
                .ToArray();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                paymentCards);

            // Act
            var paymentCardToFind = paymentCards[numberOfCardToCheck];
            var foundedPaymentCard = account.GetCard(paymentCardToFind.Number);

            // Assert
            Assert.Multiple(() =>
            {
                foundedPaymentCard.IsSuccess.Should().BeTrue();
                foundedPaymentCard.Value.Should().Be(paymentCardToFind);
            });
        }

        [Test]
        public void Get_not_contained_payment_card_is_Failure()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(customerName);
            var paymentCards = Enumerable.Range(0, 5)
                .Select(_ => paymentCardsFakeGenerator.Generate())
                .ToArray();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                paymentCards);

            // Act
            var nonExistingPaymentCardNumber = FakePaymentCardNumbers.Valid.Generate();
            var foundedPaymentCard = account.GetCard(nonExistingPaymentCardNumber);

            // Assert
            Assert.Multiple(() =>
            {
                foundedPaymentCard.IsFailure.Should().BeTrue();
                account.PaymentCards.Should().NotContain(x => x.Number == nonExistingPaymentCardNumber);
            });
        }

        [Test]
        public void Check_for_contained_payment_card_is_Success(
            [Values(5)]int numberOfCards,
            [Values(0, 1, 2, 3, 4)]int numberOfCardToCheck)
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(customerName);
            var paymentCards = Enumerable.Range(0, numberOfCards)
                .Select(_ => paymentCardsFakeGenerator.Generate())
                .ToArray();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                paymentCards);

            // Act
            var paymentCardToCheck = paymentCards[numberOfCardToCheck];
            var hasPaymentCard = account.HasCard(paymentCardToCheck);

            // Assert
            Assert.Multiple(() =>
            {
                hasPaymentCard.IsSuccess.Should().BeTrue();
                account.PaymentCards.Should().Contain(paymentCardToCheck);
            });
        }

        [Test]
        public void Check_for_not_contained_payment_card_is_Failure()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(customerName);
            var paymentCards = Enumerable.Range(0, 5)
                .Select(_ => paymentCardsFakeGenerator.Generate())
                .ToArray();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                paymentCards);

            // Act
            var nonExistingPaymentCard = paymentCardsFakeGenerator.Generate();
            var hasPaymentCard = account.HasCard(nonExistingPaymentCard);

            // Assert
            Assert.Multiple(() =>
            {
                hasPaymentCard.IsFailure.Should().BeTrue();
                account.PaymentCards.Should().NotContain(nonExistingPaymentCard);
            });
        }

        [Test]
        public void Delete_contained_payment_card_is_Success(
            [Values(5)]int numberOfCards,
            [Values(0, 1, 2, 3, 4)]int numberOfCardToCheck)
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(customerName);
            var paymentCards = Enumerable.Range(0, numberOfCards)
                .Select(_ => paymentCardsFakeGenerator.Generate())
                .ToArray();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                paymentCards);

            // Act
            var paymentCardToDelete = paymentCards[numberOfCardToCheck];
            var deleteCardResult = account.DeleteCard(paymentCardToDelete);

            // Assert
            Assert.Multiple(() =>
            {
                deleteCardResult.IsSuccess.Should().BeTrue();
                account.PaymentCards.Should().NotContain(paymentCardToDelete);
            });
        }

        [Test]
        public void Delete_contained_payment_card_by_number_is_Success(
            [Values(5)]int numberOfCards,
            [Values(0, 1, 2, 3, 4)]int numberOfCardToCheck)
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(customerName);
            var paymentCards = Enumerable.Range(0, numberOfCards)
                .Select(_ => paymentCardsFakeGenerator.Generate())
                .ToArray();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                paymentCards);

            // Act
            var paymentCardToDelete = paymentCards[numberOfCardToCheck];
            var deleteCardResult = account.DeleteCard(paymentCardToDelete.Number);

            // Assert
            Assert.Multiple(() =>
            {
                deleteCardResult.IsSuccess.Should().BeTrue();
                account.PaymentCards.Should().NotContain(paymentCardToDelete);
            });
        }
        #endregion

        #region Bind card
        [Test]
        public void Binding_of_first_card_is_Success()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                Array.Empty<PaymentCard>());

            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(customerName);
            var paymentCard = paymentCardsFakeGenerator.Generate();

            // Act
            var bindResult = account.BindCard(paymentCard, DateTimeOffset.UtcNow);

            // Assert
            Assert.Multiple(() =>
            {
                bindResult.IsSuccess.Should().BeTrue();
                account.PaymentCards.Should().Contain(paymentCard);
            });
        }

        [Test]
        public void Binding_of_next_card_is_Success()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();

            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(customerName);
            var paymentCards = paymentCardsFakeGenerator.Generate(5);
            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                paymentCards);

            var paymentCard = paymentCardsFakeGenerator.Generate();

            // Act
            var bindResult = account.BindCard(paymentCard, paymentCards.Max(x => x.ExpirationDate).AddMonths(2));

            // Assert
            Assert.Multiple(() =>
            {
                bindResult.IsSuccess.Should().BeTrue();
                account.PaymentCards.Should().Contain(paymentCard);
            });
        }

        [Test]
        public void Binding_of_next_card_at_forbidden_time_is_Failure()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();

            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(customerName);
            var paymentCards = paymentCardsFakeGenerator.Generate(5);
            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                paymentCards);

            var paymentCard = paymentCardsFakeGenerator.Generate();

            // Act
            var bindResult = account.BindCard(paymentCard, paymentCards.Max(x => x.ExpirationDate).AddDays(1));

            // Assert
            Assert.Multiple(() =>
            {
                bindResult.IsFailure.Should().BeTrue();
                account.PaymentCards.Should().NotContain(paymentCard);
            });
        }

        [Test]
        public void Binding_of_card_with_other_customer_name_is_Failure()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.One,
                Array.Empty<PaymentCard>());

            var otherCustomerName = FakeCustomerNames.Valid.Generate();
            var paymentCardsFakeGenerator = FakePaymentCards.ValidForCustomer(otherCustomerName);
            var paymentCard = paymentCardsFakeGenerator.Generate();

            // Act
            var bindResult = account.BindCard(paymentCard, DateTimeOffset.UtcNow);

            // Assert
            Assert.Multiple(() =>
            {
                bindResult.IsFailure.Should().BeTrue();
                account.PaymentCards.Should().NotContain(paymentCard);
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
            var customerName = FakeCustomerNames.Valid.Generate();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                balance,
                Array.Empty<PaymentCard>());

            // Act
            var withdrawResult = account.Withdraw(amount);

            // Assert
            Assert.Multiple(() =>
            {
                withdrawResult.IsSuccess.Should().BeTrue();
                account.Balance.Should().Be(expectedBalance);
            });
        }

        [Test, Sequential]
        public void Can_not_withdraw_when_not_enough_money(
            [Values(5, 10)] decimal balance,
            [Values(6, 11)] decimal amount)
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                balance,
                Array.Empty<PaymentCard>());

            // Act
            var withdrawResult = account.Withdraw(amount);

            // Assert
            Assert.Multiple(() =>
            {
                withdrawResult.IsFailure.Should().BeTrue();
                account.Balance.Should().Be(balance);
            });
        }

        [Test]
        public void Deposit_is_always_successful()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();

            var account = Account.Create(
                Faker.Random.Guid(),
                customerName,
                decimal.Zero,
                Array.Empty<PaymentCard>());

            var amount = Faker.Random.Decimal();

            // Act
            account.Deposit(amount);

            // Assert
            account.Balance.Should().Be(amount);
        }
        #endregion
    }
}
