using System.Linq;
using AtmSimulator.Web.Database;
using AtmSimulator.Web.Models.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Database.TransactionTests
{
    [TestFixture]
    public class SqlAtmRepositoryTransactionScopeTests : BaseTransactionScopeTest
    {
        [Test]
        public void Existing_Atm_is_returned_from_repository()
        {
            // Arrange
            var atm = FakeAtms.Valid.Generate();
            var dal = atm.ToDal();

            GlobalTransactionScopeTestSetUp.Context.Atms.Add(dal);

            GlobalTransactionScopeTestSetUp.Context.SaveChanges();

            // Act
            var dalFromRepo = GlobalTransactionScopeTestSetUp.AtmRepository.Get(atm.Id);

            // Assert
            dalFromRepo.HasValue.Should().BeTrue();

            dalFromRepo.Value.Should().Be(atm);
        }

        [Test]
        public void All_Atms_are_returned_from_repository()
        {
            // Arrange
            var atms = FakeAtms.Valid.Generate(10);
            var dals = atms.Select(x => x.ToDal()).ToArray();

            GlobalTransactionScopeTestSetUp.Context.Atms.AddRange(dals);

            GlobalTransactionScopeTestSetUp.Context.SaveChanges();

            // Act
            var dalsFromRepo = GlobalTransactionScopeTestSetUp.AtmRepository.GetAll();

            // Assert
            dalsFromRepo.Should().BeEquivalentTo(dals);
        }

        [Test]
        public void Atm_is_registered()
        {
            // Arrange
            var atm = FakeAtms.Valid.Generate();
            var dal = atm.ToDal();

            // Act
            var registerResult = GlobalTransactionScopeTestSetUp.AtmRepository.Register(atm);

            // Assert
            registerResult.IsSuccess.Should().BeTrue();

            var registeredDal = GlobalTransactionScopeTestSetUp.Context.Atms.First(x => x.Id == atm.Id);

            registeredDal.Should().BeEquivalentTo(dal);
        }

        [Test]
        public void Atm_is_updated()
        {
            // Arrange
            var originalAtm = FakeAtms.Valid.Generate();
            var originalDal = originalAtm.ToDal();

            GlobalTransactionScopeTestSetUp.Context.Atms.Add(originalDal);

            GlobalTransactionScopeTestSetUp.Context.SaveChanges();

            var updatedAtm = Atm.Create(originalAtm.Id, decimal.One);
            var updatedDal = updatedAtm.ToDal();

            // Act
            var updateResult = GlobalTransactionScopeTestSetUp.AtmRepository.Update(updatedAtm);

            // Assert
            updateResult.IsSuccess.Should().BeTrue();

            var registeredDal = GlobalTransactionScopeTestSetUp.Context.Atms.First(x => x.Id == updatedAtm.Id);

            registeredDal.Should().BeEquivalentTo(updatedDal);
        }
    }
}
