using System.Globalization;
using System.Net.Http;
using Flurl;
using TechTalk.SpecFlow;

namespace AtmSimulator.FunctionalTests.Bdd.Steps
{
    [Binding]
    [Scope(Feature = "TransferToAnotherCustomer")]
    public class TransferToAnotherCustomerSteps : TransferFeaturesSteps
    {
        [When(@"(.*) transfer to (.*) amount (.*)")]
        public async System.Threading.Tasks.Task TransferToAnotherCustomerAsync(string sender, string recipient, decimal amount)
        {
            var senderPaymentCardNumber = CustomersPaymentCards[sender];
            var recipientPaymentCardNumber = CustomersPaymentCards[recipient];

            var content = new StringContent(string.Empty);

            var requestUri = string.Empty
                .AppendPathSegment("api")
                .AppendPathSegment("v1")
                .AppendPathSegment("transfers")
                .AppendPathSegment("payment-cards")
                .AppendPathSegment(senderPaymentCardNumber)
                .AppendPathSegment("payment-cards")
                .AppendPathSegment(recipientPaymentCardNumber)
                .SetQueryParam("amount", amount.ToString(CultureInfo.InvariantCulture))
                .ToString();

            var response = await HttpClient.PostAsync(requestUri, content);

            response.EnsureSuccessStatusCode();
        }
    }
}
