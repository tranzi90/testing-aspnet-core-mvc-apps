using System;
using System.Linq;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Dawn;

namespace AtmSimulator.Web.Models.Domain
{
    public sealed class Account : ValueObject
    {
        public Guid Id { get; }

        public CustomerName CustomerName { get; set; }

        public decimal Balance { get; private set; }

        private readonly List<PaymentCard> _paymentCards;
        public IReadOnlyCollection<PaymentCard> PaymentCards => _paymentCards.AsReadOnly();

        private Account(
            Guid id,
            CustomerName customerName,
            decimal balance,
            IReadOnlyCollection<PaymentCard> paymentCards)
        {
            Id = Guard.Argument(id, nameof(id)).NotDefault();
            CustomerName = Guard.Argument(customerName, nameof(customerName)).NotNull();
            Balance = Guard.Argument(balance, nameof(balance)).NotNegative();
            _paymentCards = new List<PaymentCard>(Guard.Argument(paymentCards, nameof(paymentCards)).NotNull().Value);
        }

        public static Account Create(
            Guid id,
            CustomerName customerName,
            decimal balance,
            IReadOnlyCollection<PaymentCard> paymentCards)
            => new Account(id, customerName, balance, paymentCards);

        public static Result<Account> TryCreate(
            Guid id,
            CustomerName customerName,
            decimal balance)
            => Validate(id, balance)
            .Map(() => new Account(id, customerName, balance, Array.Empty<PaymentCard>()));

        public static Account CreateForCustomer(
            Guid id,
            CustomerName customerName)
            => new Account(id, customerName, decimal.Zero, Array.Empty<PaymentCard>());

        public static Result Validate(
            Guid id,
            decimal balance)
            => Result.Success()
            .Ensure(() => id != Guid.Empty, "Account's id MUST not be default.")
            .Ensure(() => balance >= 0, "Account's balance MUST be greater than or equal to zero.");

        public Result<PaymentCard> GetCard(PaymentCardNumber paymentCardNumber)
            => _paymentCards
            .TryFirst(x => x.Number == paymentCardNumber)
            .ToResult("Account doesn't have card with such number.");

        public Result HasCard(PaymentCard paymentCard)
            => Result.SuccessIf(_paymentCards.Contains(paymentCard), "Account doesn't have such payment card.");

        public Result DeleteCard(PaymentCardNumber paymentCardNumber)
            => GetCard(paymentCardNumber)
            .Bind(DeleteCard);

        public Result DeleteCard(PaymentCard paymentCard)
            => HasCard(paymentCard)
            .Tap(() => _paymentCards.Remove(paymentCard));

        /// <summary>
        /// Можно привязать карту, если она первая, ИЛИ если с момента выпуска последней карты прошёл месяц
        /// </summary>
        public Result BindCard(PaymentCard paymentCard, DateTimeOffset now)
            => Result.Success()
            .Ensure(() =>
            {
                if (_paymentCards.Count == 0)
                {
                    return true;
                }

                var lastIssuedExpirationDate = _paymentCards.Max(paymentCard => paymentCard.ExpirationDate);
                var forbiddenTimeToBind = lastIssuedExpirationDate.AddMonths(1);
                return forbiddenTimeToBind.Year <= now.Year
                        && forbiddenTimeToBind.Month < now.Month;
            },
                "Can bind only one card per month.")
            .Ensure(() => Balance > 0, "Can't bind payment card to account with zero balance.")
            .Ensure(() => paymentCard.CardholderName == CustomerName, "Payment card MUST be bind to its own cardholder's account only.")
            .Tap(() => _paymentCards.Add(paymentCard));

        private bool CanWithdraw(decimal amount)
            => Balance - amount >= 0;

        public Result Withdraw(decimal amount)
            => Result.SuccessIf(CanWithdraw(amount), "Account has not enough money.")
            .Tap(() => Balance -= amount);

        public void Deposit(decimal amount)
            => Balance += amount;

        public override string ToString()
            => $"📇: Id=[{Id}]. Balance=[{Balance.ToString("C2")}].";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
        }
    }
}
