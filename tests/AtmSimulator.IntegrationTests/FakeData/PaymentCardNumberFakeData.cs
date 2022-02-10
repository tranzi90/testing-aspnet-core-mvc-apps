using AtmSimulator.Web.Models.Domain;
using Bogus;

namespace AtmSimulator.IntegrationTests
{
    public class PaymentCardNumberFakeData
    {
        public PaymentCardNumberFakeData(int seed)
        {
            Valid = Valid.UseSeed(seed);
        }

        public Faker<PaymentCardNumber> Valid { get; private set; }
            = new Faker<PaymentCardNumber>("uk")
                .CustomInstantiator(f => PaymentCardNumber.Create(
                    f.Random.Short(1, PaymentCardNumber.MaximumNumberGroup),
                    f.Random.Short(1, PaymentCardNumber.MaximumNumberGroup),
                    f.Random.Short(1, PaymentCardNumber.MaximumNumberGroup),
                    f.Random.Short(1, PaymentCardNumber.MaximumNumberGroup)));
    }
}
