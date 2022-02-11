using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AtmSimulator.Web.Options;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Controllers
{
    [TestFixture]
    public class ConfigurationControllerTests : BaseWebHostTest
    {
        private const string ControllerBaseAddress = "/api/v1/configurations/";

        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _httpClient = TestServer.CreateClient();

            _httpClient.BaseAddress = new Uri(_httpClient.BaseAddress, ControllerBaseAddress);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public async Task Sample_configuration_is_returned()
        {
            // Act
            var response = await _httpClient.GetStringAsync("sample");

            var sampleOptions = JsonSerializer.Deserialize<SampleOptions>(response);

            // Assert
            sampleOptions.SampleString.Should().BeNull();
            sampleOptions.SampleInt.Should().Be(-1);
        }
    }
}
