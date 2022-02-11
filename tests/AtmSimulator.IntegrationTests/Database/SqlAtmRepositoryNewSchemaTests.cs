using System.Linq;
using AtmSimulator.Web.Database;
using AtmSimulator.Web.Models.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Database.SchemaTests
{
    [TestFixture]
    public class SqlAtmRepositoryNewSchemaTests : BaseSchemaDatabaseTestSetUp
    {
        [Test]
        public void Existing_Atm_is_returned_from_repository()
        {
            // Arrange
            var atm = FakeAtms.Valid.Generate();
            var dal = atm.ToDal();

            Context.Atms.Add(dal);

            Context.SaveChanges();

            // Act
            var dalFromRepo = AtmRepository.Get(atm.Id);

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

            Context.Atms.AddRange(dals);

            Context.SaveChanges();

            // Act
            var dalsFromRepo = AtmRepository.GetAll();

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
            var registerResult = AtmRepository.Register(atm);

            // Assert
            registerResult.IsSuccess.Should().BeTrue();

            var registeredDal = Context.Atms.First(x => x.Id == atm.Id);

            registeredDal.Should().BeEquivalentTo(dal);
        }

        [Test]
        public void Atm_is_updated()
        {
            // Arrange
            var originalAtm = FakeAtms.Valid.Generate();
            var originalDal = originalAtm.ToDal();

            Context.Atms.Add(originalDal);

            Context.SaveChanges();

            var updatedAtm = Atm.Create(originalAtm.Id, decimal.One);
            var updatedDal = updatedAtm.ToDal();

            // Act
            var updateResult = AtmRepository.Update(updatedAtm);

            // Assert
            updateResult.IsSuccess.Should().BeTrue();

            var registeredDal = Context.Atms.First(x => x.Id == updatedAtm.Id);

            registeredDal.Should().BeEquivalentTo(updatedDal);
        }
    }
}