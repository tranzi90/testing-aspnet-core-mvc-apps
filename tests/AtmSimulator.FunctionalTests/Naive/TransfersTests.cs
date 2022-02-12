using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AtmSimulator.Web.Dtos;
using FluentAssertions;
using Flurl;
using NUnit.Framework;

namespace AtmSimulator.FunctionalTests
{
    [TestFixture]
    public class TransfersTests : BaseWebHostTests
    {
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _httpClient = TestServer.CreateClient();

            var data = new
            {
                sub = Guid.NewGuid(),
                role = new[] { "Admin" }
            };

            _httpClient.SetFakeBearerToken((object)data);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public async Task Given_RegisteredCustomerAliceWithOneThousandCashAndAtmWithBalanceZero_When_AliceDepositToAtmOneThousand_Then_AliceCashIsZeroAndAliceAccountBalanceIsOneThousandAndAtmBalanceIsOneThousand()
        {
            const string Alice = "Alice";
            const decimal AliceCash = 1000M;
            const decimal AtmBalance = decimal.Zero;

            // Given
            await RegisterCustomer(Alice, AliceCash);

            var aliceCard = await IssueNewPaymentCard(Alice);

            var atmId = await RegisterAtm(AtmBalance);

            // When
            await DepositToAtm(aliceCard, atmId, AliceCash);

            // Then
            var aliceCash = await CheckCash(Alice);
            var aliceAccountBalance = await CheckAccountBalance(aliceCard);
            var atmBalance = await CheckAtmBalance(atmId);

            aliceCash.Should().Be(AtmBalance);
            aliceAccountBalance.Should().Be(AliceCash);
            atmBalance.Should().Be(AliceCash);
        }

        [Test]
        public async Task Given_RegisteredCustomerAliceWithOneThousandBalanceAndRegisteredCustomerBobWithOneHundredBalance_When_AliceTransfersToBobOneHundred_Then_AliceBalanceIsNineHundredAndBobBalanceIsTwoHundred()
        {
            const string Alice = "Alice";
            const string Bob = "Bob";

            const decimal AliceCash = 1000M;
            const decimal BobCash = 100M;

            // Given
            await RegisterCustomer(Alice, AliceCash);
            await RegisterCustomer(Bob, BobCash);

            var aliceCard = await IssueNewPaymentCard(Alice);
            var bobCard = await IssueNewPaymentCard(Bob);

            var atmId = await RegisterAtm();

            await DepositToAtm(aliceCard, atmId, AliceCash);
            await DepositToAtm(bobCard, atmId, BobCash);

            // When
            await TransferToAnotherCustomer(aliceCard, bobCard, 100M);

            // Then
            var aliceAccountBalance = await CheckAccountBalance(aliceCard);
            var bobAccountBalance = await CheckAccountBalance(bobCard);

            aliceAccountBalance.Should().Be(900M);
            bobAccountBalance.Should().Be(200M);
        }

        [Test]
        public async Task Given_RegisteredCustomerAliceWithZeroCashAndAtmWithOneThousandBalance_When_AliceWithdrawFromAtmOneThousand_Then_AliceCashIsOneThousandAndAliceAccountBalanceIsZeroAndAtmBalanceIsZero()
        {
            const string Alice = "Alice";
            const decimal AliceCash = 1000M;
            const decimal AtmBalance = decimal.Zero;

            // Given
            await RegisterCustomer(Alice, AliceCash);

            var aliceCard = await IssueNewPaymentCard(Alice);

            var atmId = await RegisterAtm(AtmBalance);

            await DepositToAtm(aliceCard, atmId, AliceCash);

            // When
            await WithdrawFromAtm(aliceCard, atmId, AliceCash);

            // Then
            var aliceCash = await CheckCash(Alice);
            var aliceAccountBalance = await CheckAccountBalance(aliceCard);
            var atmBalance = await CheckAtmBalance(atmId);

            aliceCash.Should().Be(AliceCash);
            aliceAccountBalance.Should().Be(AtmBalance);
            atmBalance.Should().Be(AtmBalance);
        }

        private async Task RegisterCustomer(string name, decimal cash)
        {
            var request = new RegisterCustomerRequestDto
            {
                CustomerName = name,
                Cash = cash,
            };

            var serializedRequest = JsonSerializer.Serialize(request, options: SerializerOptions);
            var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/customers", content);

            response.EnsureSuccessStatusCode();
        }

        private async Task<string> IssueNewPaymentCard(string name)
        {
            var content = new StringContent(string.Empty);

            var response = await _httpClient.PostAsync($"api/v1/customers/{name}/payment-cards", content);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var responseModel = JsonSerializer.Deserialize<IssuedNewPaymentCardResponseDto>(json, SerializerOptions);

            return responseModel.PaymentCardNumber;
        }

        private async Task TransferToAnotherCustomer(
            string senderPaymentCardNumber,
            string recipientPaymentCardNumber,
            decimal amount)
        {
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
                
            var response = await _httpClient.PostAsync(requestUri, content);

            response.EnsureSuccessStatusCode();
        }

        private async Task DepositToAtm(string paymentCardNumber, Guid atmId, decimal amount)
        {
            var content = new StringContent(string.Empty);

            var requestUri = string.Empty
                .AppendPathSegment("api")
                .AppendPathSegment("v1")
                .AppendPathSegment("transfers")
                .AppendPathSegment("payment-cards")
                .AppendPathSegment(paymentCardNumber)
                .AppendPathSegment("atms")
                .AppendPathSegment(atmId.ToString())
                .SetQueryParam("amount", amount.ToString(CultureInfo.InvariantCulture))
                .ToString();

            var response = await _httpClient.PostAsync(requestUri, content);

            response.EnsureSuccessStatusCode();
        }

        private async Task WithdrawFromAtm(string paymentCardNumber, Guid atmId, decimal amount)
        {
            var content = new StringContent(string.Empty);

            var requestUri = string.Empty
                .AppendPathSegment("api")
                .AppendPathSegment("v1")
                .AppendPathSegment("transfers")
                .AppendPathSegment("atms")
                .AppendPathSegment(atmId.ToString())
                .AppendPathSegment("payment-cards")
                .AppendPathSegment(paymentCardNumber)
                .SetQueryParam("amount", amount.ToString(CultureInfo.InvariantCulture))
                .ToString();

            var response = await _httpClient.PostAsync(requestUri, content);

            response.EnsureSuccessStatusCode();
        }

        private async Task<Guid> RegisterAtm(decimal balance = 0M)
        {
            var content = new StringContent(string.Empty);

            var response = await _httpClient.PostAsync($"api/v1/atms?balance={balance.ToString(CultureInfo.InvariantCulture)}", content);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var responseModel = JsonSerializer.Deserialize<RegisteredAtmResponseDto>(json, SerializerOptions);

            return responseModel.AtmId;
        }

        private async Task<decimal> CheckCash(string customerName)
        {
            var response = await _httpClient.GetStringAsync($"api/v1/customers/{customerName}/cash");

            return decimal.Parse(response);
        }

        private async Task<decimal> CheckAtmBalance(Guid atmId)
        {
            var response = await _httpClient.GetStringAsync($"api/v1/atms/{atmId.ToString()}/balance");

            return decimal.Parse(response);
        }

        private async Task<decimal> CheckAccountBalance(string paymentCardNumber)
        {
            var response = await _httpClient.GetStringAsync($"api/v1/accounts/balance?paymentCardNumber={paymentCardNumber}");

            return decimal.Parse(response);
        }
    }
}
