using System.Globalization;
using System.Net.Http;
using Flurl;
using TechTalk.SpecFlow;

namespace AtmSimulator.FunctionalTests.Bdd.Steps
{
    [Binding]
    [Scope(Feature = "WithdrawFromAtm")]
    public class WithdrawFromAtmSteps : TransferFeaturesSteps
    {
        [When(@"(.*) withdraw from ATM (.*)")]
        public async System.Threading.Tasks.Task WithdrawFromAtmAsync(string name, decimal amount)
        {
            var paymentCardNumber = CustomersPaymentCards[name];

            var content = new StringContent(string.Empty);

            var requestUri = string.Empty
                .AppendPathSegment("api")
                .AppendPathSegment("v1")
                .AppendPathSegment("transfers")
                .AppendPathSegment("atms")
                .AppendPathSegment(AtmId.ToString())
                .AppendPathSegment("payment-cards")
                .AppendPathSegment(paymentCardNumber)
                .SetQueryParam("amount", amount.ToString(CultureInfo.InvariantCulture))
                .ToString();

            var response = await HttpClient.PostAsync(requestUri, content);

            response.EnsureSuccessStatusCode();
        }
    }
}
