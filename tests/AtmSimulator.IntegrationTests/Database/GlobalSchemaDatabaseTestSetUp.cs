using AtmSimulator.Web.Database;
using AtmSimulator.Web.Models.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Database.SchemaTests
{
    [SetUpFixture]
    public class GlobalSchemaDatabaseTestSetUp
    {
        public static readonly string ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database={nameof(GlobalSchemaDatabaseTestSetUp)};Trusted_Connection=True;MultipleActiveResultSets=true";

        private AtmSimulatorDbContext _baseContext;

        [OneTimeSetUp]
        public void TestOneTimeSetUp()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AtmSimulatorDbContext>(builder =>
                {
                    builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    builder.UseSqlServer(ConnectionString)
                        .ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>()
                        .ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>();
                })
                .BuildServiceProvider();

            _baseContext = serviceProvider.GetRequiredService<AtmSimulatorDbContext>();
            _baseContext.Database.EnsureDeleted();
            _baseContext.Database.EnsureCreated();
        }

        [OneTimeTearDown]
        public void TestOneTimeTearDown()
        {
            _baseContext.Database.EnsureDeleted();

            _baseContext.Dispose();
        }
    }

    public abstract class BaseSchemaDatabaseTestSetUp : BaseTest
    {
        protected AtmSimulatorDbContext Context { get; private set; }

        protected SqlAtmRepository AtmRepository { get; private set; }

        protected SqlCustomerRepository CustomerRepository { get; private set; }

        [SetUp]
        public void TestSetUp()
        {
            var currentTestName = TestContext.CurrentContext.Test.Name;

            var services = new ServiceCollection()
                .AddDbContext<AtmSimulatorDbContext>(builder =>
                {
                    builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    builder.UseSqlServer(
                        GlobalSchemaDatabaseTestSetUp.ConnectionString,
                        b => b.MigrationsHistoryTable("__EFMigrationsHistory", currentTestName))
                        .ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>()
                        .ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>();
                })
                .AddSingleton<IDbContextSchema>(new DbContextSchema(currentTestName));

            var serviceProvider = services.BuildServiceProvider();

            Context = serviceProvider.GetRequiredService<AtmSimulatorDbContext>();
            Context.Database.Migrate();

            AtmRepository = new SqlAtmRepository(Context);
            CustomerRepository = new SqlCustomerRepository(Context);
        }

        [TearDown]
        public void TestTearDown()
        {
            Context.Dispose();
        }
    }
}
