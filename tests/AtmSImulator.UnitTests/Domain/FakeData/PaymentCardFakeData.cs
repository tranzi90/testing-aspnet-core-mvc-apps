using System;
using AtmSimulator.Web.Models.Domain;
using Bogus;

namespace AtmSimulator.UnitTests
{
    public class PaymentCardFakeData
    {
        private readonly int _seed;
        private readonly PaymentCardNumberFakeData _paymentCardNumberFakeData;

        public PaymentCardFakeData(int seed, PaymentCardNumberFakeData paymentCardNumberFakeData)
        {
            _seed = seed;
            _paymentCardNumberFakeData = paymentCardNumberFakeData;
        }

        public Faker<PaymentCard> ValidForCustomer(CustomerName customerName)
            => new Faker<PaymentCard>("uk")
                .UseSeed(_seed)
                .CustomInstantiator(f => PaymentCard.Create(
                    _paymentCardNumberFakeData.Valid.Generate(),
                    customerName,
                    f.Date.FutureOffset(refDate: DateTimeOffset.UtcNow + TimeSpan.FromDays(180)),
                    f.Random.Short(0)));
    }
}
