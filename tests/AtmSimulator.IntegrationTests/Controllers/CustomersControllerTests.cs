using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AtmSimulator.Web.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Controllers
{
    [TestFixture]
    public class CustomersControllerTests : BaseTest
    {
        private const string ControllerBaseAddress = "/api/v1/customers/";

        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _httpClient = Host.GetTestServer().CreateClient();

            _httpClient.BaseAddress = new Uri(_httpClient.BaseAddress, ControllerBaseAddress);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        [Test]
        [TestCase("Тарас Іванишин/cash")]
        public async Task All_Get_routes_are_accessible(string route)
        {
            // Act
            var response = await _httpClient.GetAsync(route);

            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }

        [Test]
        [TestCase("")]
        public async Task All_Post_routes_are_accessible(string route)
        {
            // Arrange
            var content = new StringContent(string.Empty);

            // Act
            var response = await _httpClient.PostAsync(route, content);

            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Register_customer_request_is_valid()
        {
            // Arrange
            var request = new RegisterCustomerRequestDto
            {
                CustomerName = FakeCustomerNames.Valid.Generate().Name,
                Cash = decimal.One,
            };

            var serializerOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            var serializedRequest = JsonSerializer.Serialize(request, options: serializerOptions);
            var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("", content);

            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Admin_can_register_customer()
        {
            // Arrange
            var data = new 
            {
                sub = Guid.NewGuid(),
                role = new[] { "Admin" }
            };

            _httpClient.SetFakeBearerToken((object)data);

            var request = new RegisterCustomerRequestDto
            {
                CustomerName = FakeCustomerNames.Valid.Generate().Name,
                Cash = decimal.One,
            };

            var serializerOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            var serializedRequest = JsonSerializer.Serialize(request, options: serializerOptions);
            var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task Anonymous_user_can_not_register_customer()
        {
            // Arrange
            var content = new StringContent(string.Empty);

            // Act
            var response = await _httpClient.PostAsync("", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
