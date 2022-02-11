using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Controllers
{
    [TestFixture]
    public class HomeControllerTests : BaseWebHostTest
    {
        private const string ControllerBaseAddress = "/Home/";

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
        public async Task Home_viewmodel_is_returned()
        {
            // Arrange
            var expectedAtms = FakeAtmRepository
                .GetAll()
                .Select(x => new 
                { 
                    Id = x.Id.ToString(),
                    Balance = x.Balance.ToString("C2"),
                })
                .ToArray();

            var expectedTotalCount = expectedAtms.Length.ToString();

            // Act
            var response = await _httpClient.GetStringAsync("Index");

            // Assert
            var parser = new HtmlParser();

            var document = parser.ParseDocument(response);

            var table = document.QuerySelector("#atms");

            var atmsInView = table.QuerySelectorAll("tr")
                .Select(tr => 
                {
                    var tds = tr.Children;

                    var id = tds[0].TextContent.Trim(' ', '\r', '\n');
                    var balance = tds[1].TextContent.Trim(' ', '\r', '\n');

                    return new
                    {
                        Id = id,
                        Balance = balance,
                    };
                });

            var totalCountViewData = document.QuerySelector("#total-count-viewdata").TextContent;
            var totalCountViewBag = document.QuerySelector("#total-count-viewbag").TextContent;

            Assert.Multiple(() =>
            {
                atmsInView.Should().BeEquivalentTo(expectedAtms);

                totalCountViewData.Should().Be(expectedTotalCount);
                totalCountViewBag.Should().Be(expectedTotalCount);
            });
        }
    }
}
