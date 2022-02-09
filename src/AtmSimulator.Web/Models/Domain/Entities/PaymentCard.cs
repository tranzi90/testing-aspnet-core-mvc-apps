using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Dawn;

namespace AtmSimulator.Web.Models.Domain
{
    public sealed class PaymentCard : ValueObject
    {
        private PaymentCard(
            PaymentCardNumber number,
            CustomerName cardholderName,
            DateTimeOffset expirationDate,
            short securityCode)
        {
            Number = Guard.Argument(number, nameof(number)).NotNull();
            CardholderName = Guard.Argument(cardholderName, nameof(cardholderName)).NotNull();
            ExpirationDate = Guard.Argument(expirationDate, nameof(expirationDate)).NotDefault();
            SecurityCode = Guard.Argument(securityCode, nameof(securityCode), secure: true).Positive();
        }

        public PaymentCardNumber Number { get; }

        public CustomerName CardholderName { get; set; }

        public DateTimeOffset ExpirationDate { get; }

        public short SecurityCode { get; }

        public static PaymentCard Create(
            PaymentCardNumber number,
            CustomerName cardholderName,
            DateTimeOffset expirationDate,
            short securityCode)
            => new PaymentCard(
                number,
                cardholderName,
                expirationDate,
                securityCode);

        public static Result<PaymentCard> TryCreate(
            PaymentCardNumber number,
            CustomerName cardholderName,
            IDateTimeProvider dateTimeProvider,
            DateTimeOffset expirationDate,
            short securityCode)
            => Validate(dateTimeProvider, expirationDate, securityCode)
            .Map(() => Create(number, cardholderName, expirationDate, securityCode));

        public static CSharpFunctionalExtensions.Result Validate(
            IDateTimeProvider dateTimeProvider,
            DateTimeOffset expirationDate,
            short securityCode)
            => CSharpFunctionalExtensions.Result.Success()
            .Ensure(() => expirationDate > dateTimeProvider.UtcNow, "Payment card's expiration date MUST be greater than current datetime.")
            .Ensure(() => securityCode > 0, "Payment card's security number MUST be greater than zero.");

        public override string ToString()
            => $"💳: Number=[{Number}]. CardholderName=[{CardholderName}]. ExpirationDate=[{ExpirationDate.ToString("MM/y")}].";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
        }
    }
}
