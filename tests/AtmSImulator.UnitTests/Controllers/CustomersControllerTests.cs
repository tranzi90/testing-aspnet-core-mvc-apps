using AtmSimulator.Web.Controllers;
using AtmSimulator.Web.Dtos;
using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;


namespace AtmSimulator.UnitTests.Controllers
{
    [TestFixture]
    public class CustomersControllerTests : BaseTest
    {
        [Test]
        public void Customer_is_registered()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var customerCash = decimal.One;
            var customer = Customer.Create(
                customerName,
                customerCash,
                Faker.Random.Guid());

            var financialInformationService = Substitute.For<IFinancialInformationService>();
            var financialInstitutionService = Substitute.For<IFinancialInstitutionService>();

            financialInstitutionService.RegisterCustomer(customerName, customerCash).Returns(Result.Success(customer));

            var customersController = new CustomersController(
                financialInformationService,
                financialInstitutionService);

            var registerCustomerRequest = new RegisterCustomerRequestDto
            {
                CustomerName = customerName.Name,
                Cash = customerCash,
            };

            // Act
            var registeredCustomerResponse = customersController.RegisterCustomer(registerCustomerRequest);

            // Asssert
            var registeredCustomerDto = registeredCustomerResponse.GetInnerValue();

            Assert.Multiple(() =>
            {
                registeredCustomerDto.CustomerName.Should().Be(customerName.Name);
                registeredCustomerDto.Cash.Should().Be(customer.Cash);
                registeredCustomerDto.AccountId.Should().Be(customer.AccountId);

                financialInstitutionService.Received(1).RegisterCustomer(customerName, customerCash);
            });
        }

        [Test]
        public void Customer_get_his_cash()
        {
            // Arrange
            var customerName = FakeCustomerNames.Valid.Generate();
            var customerCash = decimal.One;
            var customer = Customer.Create(
                customerName,
                customerCash,
                Faker.Random.Guid());

            var financialInformationService = Substitute.For<IFinancialInformationService>();
            var financialInstitutionService = Substitute.For<IFinancialInstitutionService>();

            financialInformationService.CheckCustomerCash(customerName).Returns(Result.Success(customerCash));

            var customersController = new CustomersController(
                financialInformationService,
                financialInstitutionService);

            // Act
            var cashResponse = customersController.GetCash(customerName.Name);

            // Asssert
            var cash = cashResponse.GetInnerValue();

            Assert.Multiple(() =>
            {
                cash.Should().Be(customer.Cash);

                financialInformationService.Received(1).CheckCustomerCash(customerName);
            });
        }
    }
}
