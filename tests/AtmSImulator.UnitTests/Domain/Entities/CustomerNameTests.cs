using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Domain.Entities
{
    [TestFixture]
    public class CustomerNameTests : BaseTest
    {
        #region Creation
        [Test]
        public void Customer_name_name_with_invalid_arguments_throw_exceptions()
        {
            // Arrange
            Action createWithNullName = () => CustomerName.Create(null);
            Action createWithEmptyName = () => CustomerName.Create(string.Empty);
            Action createWithWhitespaceName = () => CustomerName.Create("   ");

            // Assert
            Assert.Multiple(() =>
            {
                createWithNullName.Should().Throw<ArgumentNullException>();
                createWithEmptyName.Should().Throw<ArgumentException>();
                createWithWhitespaceName.Should().Throw<ArgumentException>();
            });
        }

        [Test]
        [TestCase("Антоній Скиба", "АНТОНІЙ СКИБА")]
        [TestCase("John Smith", "JOHN SMITH")]
        public void Customer_name_with_valid_arguments_is_created(string name, string normalizedName)
        {
            // Arrange
            Func<CustomerName> createCustomerName = () => CustomerName.Create(name);

            // Assert
            createCustomerName.Should().NotThrow();

            Assert.Multiple(() =>
            {
                var customerName = createCustomerName();

                customerName.Name.Should().Be(name);
                customerName.NormalizedName.Should().Be(normalizedName);
            });
        }

        [Test]
        public void Customer_name_with_null_name_has_Failure_result()
        {
            // Arrange
            var customerName = CustomerName.TryCreate(null);

            // Assert
            customerName.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Customer_name_with_null_name_is_invalid()
        {
            // Arrange
            var customerName = CustomerName.Validate(null);

            // Assert
            customerName.IsFailure.Should().BeTrue();
        }

        [Test]
        [TestCase("Антоній Скиба", "АНТОНІЙ СКИБА")]
        [TestCase("John Smith", "JOHN SMITH")]
        public void Customer_name_with_valid_arguments_has_Success_result(string name, string normalizedName)
        {
            // Arrange
            Func<Result<CustomerName>> createCustomerName = () => CustomerName.TryCreate(name);

            // Assert
            createCustomerName.Should().NotThrow();

            var maybeCustomerName = createCustomerName();

            maybeCustomerName.IsSuccess.Should().BeTrue();

            Assert.Multiple(() =>
            {
                var customerName = maybeCustomerName.Value;

                customerName.Name.Should().Be(name);
                customerName.NormalizedName.Should().Be(normalizedName);
            });
        }
        #endregion
    }
}
