using System.Linq;
using AtmSimulator.Web.Database;
using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Database
{
    [TestFixture]
    public class SqlAtmRepositoryNewDatabaseTests : BaseTest
    {
        private AtmSimulatorDbContext _context;

        private SqlAtmRepository _repository;

        [SetUp]
        public void TestSetUp()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<AtmSimulatorDbContext>();

            var databaseName = nameof(SqlAtmRepositoryNewDatabaseTests);

            builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            builder.UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true")
                .UseInternalServiceProvider(serviceProvider);

            _context = new AtmSimulatorDbContext(builder.Options);
            _context.Database.EnsureDeleted();
            _context.Database.Migrate();

            _repository = new SqlAtmRepository(_context);
        }

        [TearDown]
        public void TestTearDown()
        {
            _context.Database.EnsureDeleted();

            _context.Dispose();
        }

        [Test]
        public void Existing_Atm_is_returned_from_repository()
        {
            // Arrange
            var atm = FakeAtms.Valid.Generate();
            var dal = atm.ToDal();

            _context.Atms.Add(dal);

            _context.SaveChanges();

            // Act
            var dalFromRepo = _repository.Get(atm.Id);

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

            _context.Atms.AddRange(dals);

            _context.SaveChanges();

            // Act
            var dalsFromRepo = _repository.GetAll();

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
            var registerResult = _repository.Register(atm);

            // Assert
            registerResult.IsSuccess.Should().BeTrue();

            var registeredDal = _context.Atms.First(x => x.Id == atm.Id);

            registeredDal.Should().BeEquivalentTo(dal);
        }

        [Test]
        public void Atm_is_updated()
        {
            // Arrange
            var originalAtm = FakeAtms.Valid.Generate();
            var originalDal = originalAtm.ToDal();

            _context.Atms.Add(originalDal);

            _context.SaveChanges();

            var updatedAtm = Atm.Create(originalAtm.Id, decimal.One);
            var updatedDal = updatedAtm.ToDal();

            // Act
            var updateResult = _repository.Update(updatedAtm);

            // Assert
            updateResult.IsSuccess.Should().BeTrue();

            var registeredDal = _context.Atms.First(x => x.Id == updatedAtm.Id);

            registeredDal.Should().BeEquivalentTo(updatedDal);
        }
    }
}

