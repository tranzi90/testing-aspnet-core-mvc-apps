using Microsoft.EntityFrameworkCore;

namespace AtmSimulator.Web.Database
{
    public class AtmSimulatorDbContext : DbContext, IDbContextSchema
    {
        public string Schema { get; }

        public DbSet<AtmDal> Atms { get; set; }

        public DbSet<CustomerDal> Customers { get; set; }

        public AtmSimulatorDbContext(
           DbContextOptions<AtmSimulatorDbContext> options,
           IDbContextSchema schema = null)
           : base(options)
        {
            Schema = schema?.Schema;
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AtmDalEntityConfiguration(Schema));
            modelBuilder.ApplyConfiguration(new CustomerDalEntityConfiguration(Schema));
        }
    }
}
