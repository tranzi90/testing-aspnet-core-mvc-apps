using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AtmSimulator.Web.Dtos;
using FluentAssertions;
using Flurl;
using TechTalk.SpecFlow;

namespace AtmSimulator.FunctionalTests.Bdd.Steps
{
    public abstract class TransferFeaturesSteps : BaseWebHostTests
    {
        protected readonly Dictionary<string, string> CustomersPaymentCards = new Dictionary<string, string>(0);

        protected Guid AtmId;

        protected HttpClient HttpClient { get; private set; }

        [BeforeScenario(Order = 2)]
        public async Task BeforeScenarioSteps()
        {
            HttpClient = TestServer.CreateClient();

            var data = new
            {
                sub = Guid.NewGuid(),
                role = new[] { "Admin" }
            };

            HttpClient.SetFakeBearerToken((object)data);

            await RegisterAtmInner();
        }

        [AfterScenario(Order = 0)]
        public void AfterScenarioSteps()
        {
            HttpClient.Dispose();
        }

        protected async Task DepositToAtmInner(string name, decimal amount)
        {
            var paymentCardNumber = CustomersPaymentCards[name];

            var content = new StringContent(string.Empty);

            var requestUri = string.Empty
                .AppendPathSegment("api")
                .AppendPathSegment("v1")
                .AppendPathSegment("transfers")
                .AppendPathSegment("payment-cards")
                .AppendPathSegment(paymentCardNumber)
                .AppendPathSegment("atms")
                .AppendPathSegment(AtmId.ToString())
                .SetQueryParam("amount", amount.ToString(CultureInfo.InvariantCulture))
                .ToString();

            var response = await HttpClient.PostAsync(requestUri, content);

            response.EnsureSuccessStatusCode();
        }

        [Given(@"registered customer (.*) with (.*) cash")]
        public async Task RegisterCustomerWithCash(string name, decimal cash)
        {
            await RegisterCustomerInner(name, cash);

            await IssueNewPaymentCardInner(name);
        }

        [Given(@"registered customer (.*) with (.*) on account")]
        public async Task RegisterCustomerWithAccountBalance(string name, decimal amount)
        {
            await RegisterCustomerInner(name, amount);

            await IssueNewPaymentCardInner(name);

            await DepositToAtmInner(name, amount);
        }

        [Given(@"registered ATM with (.*) balance")]
        public async Task RegisterAtmAsync(decimal balance)
        {
            await RegisterAtmInner(balance);
        }

        [Then(@"(.*) cash is (.*)")]
        public async Task CheckCashAsync(string name, decimal cash)
        {
            var response = await HttpClient.GetStringAsync($"api/v1/customers/{name}/cash");

            var customerCash = decimal.Parse(response);

            customerCash.Should().Be(cash);
        }

        [Then(@"(.*) account balance is (.*)")]
        public async Task CheckAccountBalanceAsync(string name, decimal balance)
        {
            var paymentCardNumber = CustomersPaymentCards[name];

            var response = await HttpClient.GetStringAsync($"api/v1/accounts/balance?paymentCardNumber={paymentCardNumber}");

            var accountBalance = decimal.Parse(response);

            accountBalance.Should().Be(balance);
        }

        [Then(@"ATM balance is (.*)")]
        public async Task CheckAtmBalanceAsync(decimal balance)
        {
            var response = await HttpClient.GetStringAsync($"api/v1/atms/{AtmId.ToString()}/balance");

            var atmBalance = decimal.Parse(response);

            atmBalance.Should().Be(balance);
        }

        private async Task RegisterAtmInner(decimal balance = 0M)
        {
            var content = new StringContent(string.Empty);

            var response = await HttpClient.PostAsync($"api/v1/atms?balance={balance.ToString(CultureInfo.InvariantCulture)}", content);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var responseModel = JsonSerializer.Deserialize<RegisteredAtmResponseDto>(json, SerializerOptions);

            AtmId = responseModel.AtmId;
        }

        private async Task RegisterCustomerInner(string name, decimal cash)
        {
            var request = new RegisterCustomerRequestDto
            {
                CustomerName = name,
                Cash = cash,
            };

            var serializedRequest = JsonSerializer.Serialize(request, options: SerializerOptions);
            var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync("api/v1/customers", content);

            response.EnsureSuccessStatusCode();
        }

        private async Task IssueNewPaymentCardInner(string name)
        {
            var content = new StringContent(string.Empty);

            var response = await HttpClient.PostAsync($"api/v1/customers/{name}/payment-cards", content);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var responseModel = JsonSerializer.Deserialize<IssuedNewPaymentCardResponseDto>(json, SerializerOptions);

            CustomersPaymentCards[name] = responseModel.PaymentCardNumber;
        }
    }
}
