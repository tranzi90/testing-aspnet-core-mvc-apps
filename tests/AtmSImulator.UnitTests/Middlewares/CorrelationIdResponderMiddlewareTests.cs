using System.Threading.Tasks;
using AtmSimulator.Web.Middlewares;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Middlewares
{
    [TestFixture]
    public class CorrelationIdResponderMiddlewareTests : BaseTest
    {
        [Test]
        public async Task Correlation_id_header_is_returned()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var correlationId = Faker.Random.Guid().ToString();
            httpContext.Request.Headers.Add("Respond-With-Correlation-Id", correlationId);

            var requestDelegate = Substitute.For<RequestDelegate>();

            requestDelegate.Invoke(httpContext).Returns(Task.CompletedTask);

            var middleware = new CorrelationIdResponderMiddleware(requestDelegate);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            Assert.Multiple(() =>
            {
                httpContext.Response.Headers.Should().Contain("Correlation-Id", correlationId);

                requestDelegate.Received(1).Invoke(httpContext);
            });
        }

        [Test]
        public async Task Correlation_id_header_is_not_returned()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var correlationId = Faker.Random.Guid().ToString();

            var requestDelegate = Substitute.For<RequestDelegate>();

            requestDelegate.Invoke(httpContext).Returns(Task.CompletedTask);

            var middleware = new CorrelationIdResponderMiddleware(requestDelegate);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            Assert.Multiple(() =>
            {
                httpContext.Response.Headers.Should().NotContain("Correlation-Id", correlationId);

                requestDelegate.Received(1).Invoke(httpContext);
            });
        }
    }
}
