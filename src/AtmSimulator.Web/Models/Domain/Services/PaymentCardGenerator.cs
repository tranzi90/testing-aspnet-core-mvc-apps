using System;

namespace AtmSimulator.Web.Models.Domain
{
    public class PaymentCardGenerator
    {
        private readonly IRandomGenerator _randomGenerator;

        public PaymentCardGenerator(IRandomGenerator randomGenerator)
        {
            _randomGenerator = randomGenerator;
        }

        public PaymentCard GenerateNewCard(CustomerName customerName, DateTimeOffset now)
        {
            var paymentCardNumber = PaymentCardNumber.Create(
                GeneratePaymentCardNumberGroup(),
                GeneratePaymentCardNumberGroup(),
                GeneratePaymentCardNumberGroup(),
                GeneratePaymentCardNumberGroup());
            var expirationDate = now.AddYears(1);
            var securityCode = _randomGenerator.NextPositiveShort();

            var paymentCard = PaymentCard.Create(
                paymentCardNumber,
                customerName,
                expirationDate,
                securityCode);

            return paymentCard;
        }

        private short GeneratePaymentCardNumberGroup()
            => (short)(_randomGenerator.NextPositiveShort() % PaymentCardNumber.MaximumNumberGroup);
    }
}
