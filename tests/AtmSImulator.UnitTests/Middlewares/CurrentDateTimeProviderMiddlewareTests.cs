using System.IO;
using System.Threading.Tasks;
using AtmSimulator.UnitTests.Extensions;
using AtmSimulator.Web.Middlewares;
using AtmSimulator.Web.Models.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Middlewares
{
    [TestFixture]
    public class CurrentDateTimeProviderMiddlewareTests : BaseTest
    {
        [Test]
        public async Task Current_unix_time_seconds_are_returned()
        {
            // Arrange
            var currentDateTime = Faker.Date.FutureOffset();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/current-unix-time-seconds";
            httpContext.Response.Body = new MemoryStream();

            var requestDelegate = Substitute.For<RequestDelegate>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();

            dateTimeProvider.UtcNow.Returns(currentDateTime);

            var middleware = new CurrentDateTimeProviderMiddleware(requestDelegate, dateTimeProvider);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            Assert.Multiple(() =>
            {
                var response = httpContext.Response.Body.Read();

                var expectedResponse = currentDateTime.ToUnixTimeSeconds().ToString();

                response.Should().Be(expectedResponse);

                requestDelegate.DidNotReceive().Invoke(httpContext);
            });
        }

        [Test]
        public async Task Current_unix_time_seconds_are_not_returned()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            var requestDelegate = Substitute.For<RequestDelegate>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();

            requestDelegate.Invoke(httpContext).Returns(Task.CompletedTask);

            var middleware = new CurrentDateTimeProviderMiddleware(requestDelegate, dateTimeProvider);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            Assert.Multiple(() =>
            {
                requestDelegate.Received(1).Invoke(httpContext);
            });
        }
    }
}
