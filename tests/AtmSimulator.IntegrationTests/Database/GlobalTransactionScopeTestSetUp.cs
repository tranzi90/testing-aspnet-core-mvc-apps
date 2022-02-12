using AtmSimulator.Web.Database;
using AtmSimulator.Web.Models.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Database.TransactionTests
{
    [SetUpFixture]
    public class GlobalTransactionScopeTestSetUp
    {
        public static readonly string ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database={nameof(GlobalTransactionScopeTestSetUp)};Trusted_Connection=True;MultipleActiveResultSets=true";

        private static AtmSimulatorDbContext _baseContext;

        [OneTimeSetUp]
        public void TestOneTimeSetUp()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<AtmSimulatorDbContext>();

            builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            builder.UseSqlServer(ConnectionString)
                .UseInternalServiceProvider(serviceProvider);

            _baseContext = new AtmSimulatorDbContext(builder.Options);
            _baseContext.Database.EnsureDeleted();
            _baseContext.Database.Migrate();
        }

        [OneTimeTearDown]
        public void TestOneTimeTearDown()
        {
            _baseContext.Database.EnsureDeleted();

            _baseContext.Dispose();
        }
    }

    public abstract class BaseTransactionScopeTest : BaseTest
    {
        private IDbContextTransaction _transaction;

        protected AtmSimulatorDbContext Context { get; private set; }

        protected SqlAtmRepository AtmRepository { get; private set; }

        protected SqlCustomerRepository CustomerRepository { get; private set; }

        [SetUp]
        public void TestSetUp()
        {
            var services = new ServiceCollection()
                .AddDbContext<AtmSimulatorDbContext>(builder =>
                {
                    builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    builder.UseSqlServer(GlobalTransactionScopeTestSetUp.ConnectionString);
                });

            var serviceProvider = services.BuildServiceProvider();

            Context = serviceProvider.GetRequiredService<AtmSimulatorDbContext>();
            AtmRepository = new SqlAtmRepository(Context);
            CustomerRepository = new SqlCustomerRepository(Context);

            _transaction = Context.Database
                .BeginTransaction(System.Data.IsolationLevel.Serializable);
        }

        [TearDown]
        public void TestTearDown()
        {
            _transaction.Dispose();

            Context.Dispose();
        }
    }
}
