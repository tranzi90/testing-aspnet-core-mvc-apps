using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Middlewares
{
    [TestFixture]
    public class CurrentDateTimeProviderMiddlewareTests : BaseTest
    {
        private const string MiddlewareBaseAddress = "/current-unix-time-seconds";

        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _httpClient = Host.GetTestServer().CreateClient();

            _httpClient.BaseAddress = new Uri(_httpClient.BaseAddress, MiddlewareBaseAddress);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        [Test]
        [TestCase("946782245")]
        public async Task Current_datetime_is_returned(string expectedResult)
        {
            // Act
            var response = await _httpClient.GetStringAsync("");

            // Assert
            response.Should().Be(expectedResult);
        }
    }
}
