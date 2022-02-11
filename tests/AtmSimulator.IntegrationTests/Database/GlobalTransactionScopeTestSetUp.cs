using AtmSimulator.Web.Database;
using AtmSimulator.Web.Models.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace AtmSimulator.IntegrationTests.Database.TransactionTests
{
    [SetUpFixture]
    [Parallelizable(ParallelScope.None)]
    public class GlobalTransactionScopeTestSetUp
    {
        private const string DatabaseName = nameof(GlobalTransactionScopeTestSetUp);

        public static AtmSimulatorDbContext Context { get; private set; }

        public static SqlAtmRepository AtmRepository { get; private set; }

        public static SqlCustomerRepository CustomerRepository { get; private set; }

        [OneTimeSetUp]
        public void TestOneTimeSetUp()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<AtmSimulatorDbContext>();

            builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            builder.UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database={DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true")
                .UseInternalServiceProvider(serviceProvider);

            Context = new AtmSimulatorDbContext(builder.Options);
            Context.Database.EnsureDeleted();
            Context.Database.Migrate();

            AtmRepository = new SqlAtmRepository(Context);
            CustomerRepository = new SqlCustomerRepository(Context);
        }

        [OneTimeTearDown]
        public void TestOneTimeTearDown()
        {
            Context.Database.EnsureDeleted();

            Context.Dispose();
        }
    }

    public abstract class BaseTransactionScopeTest : BaseTest
    {
        private IDbContextTransaction _transaction;

        [SetUp]
        public void TestSetUp()
        {
            DetachEntries(GlobalTransactionScopeTestSetUp.Context.Atms.Local);
            DetachEntries(GlobalTransactionScopeTestSetUp.Context.Customers.Local);

            _transaction = GlobalTransactionScopeTestSetUp.Context.Database
                .BeginTransaction(System.Data.IsolationLevel.Serializable);
        }

        private void DetachEntries<T>(LocalView<T> localView)
            where T : class
        {
            foreach (var entry in localView)
            {
                GlobalTransactionScopeTestSetUp.Context.Entry(entry).State = EntityState.Detached;
            }
        }

        [TearDown]
        public void TestTearDown()
        {
            _transaction.Dispose();
        }
    }
}
