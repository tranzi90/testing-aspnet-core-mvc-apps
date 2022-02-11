using System.Linq;
using AtmSimulator.Web.Database;
using AtmSimulator.Web.Models.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Database.SchemaTests
{
    [TestFixture]
    public class SqlCustomerRepositoryNewSchemaTests : BaseSchemaDatabaseTestSetUp
    {
        [Test]
        public void Existing_Customer_is_returned_from_repository()
        {
            // Arrange
            var customer = FakeCustomers.Valid().Generate();
            var dal = customer.ToDal();

            Context.Customers.Add(dal);

            Context.SaveChanges();

            // Act
            var dalFromRepo = CustomerRepository.Get(customer.Name);

            // Assert
            dalFromRepo.HasValue.Should().BeTrue();

            dalFromRepo.Value.Should().Be(customer);
        }

        [Test]
        public void Customer_is_registered()
        {
            // Arrange
            var customer = FakeCustomers.Valid().Generate();
            var dal = customer.ToDal();

            // Act
            var registerResult = CustomerRepository.Register(customer);

            // Assert
            registerResult.IsSuccess.Should().BeTrue();

            var registeredDal = Context.Customers.First(x => x.Name == customer.Name.Name);

            registeredDal.Should().BeEquivalentTo(dal);
        }

        [Test]
        public void Customer_is_updated()
        {
            // Arrange
            var originalCustomer = FakeCustomers.Valid().Generate();
            var originalDal = originalCustomer.ToDal();

            Context.Customers.Add(originalDal);

            Context.SaveChanges();

            var updatedCustomer = Customer.Create(originalCustomer.Name, decimal.One, Faker.Random.Guid());
            var updatedDal = updatedCustomer.ToDal();

            // Act
            var updateResult = CustomerRepository.Update(updatedCustomer);

            // Assert
            updateResult.IsSuccess.Should().BeTrue();

            var registeredDal = Context.Customers.First(x => x.Name == updatedCustomer.Name.Name);

            registeredDal.Should().BeEquivalentTo(updatedDal);
        }
    }
}