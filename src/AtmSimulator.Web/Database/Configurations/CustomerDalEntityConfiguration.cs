using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtmSimulator.Web.Database
{
    public class CustomerDalEntityConfiguration : IEntityTypeConfiguration<CustomerDal>
    {
        private readonly string _schema;

        public CustomerDalEntityConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<CustomerDal> builder)
        {
            if (!string.IsNullOrWhiteSpace(_schema))
            {
                builder.ToTable(nameof(AtmSimulatorDbContext.Customers), _schema);
            }
            else
            {
                builder.ToTable(nameof(AtmSimulatorDbContext.Customers));
            }

            builder.HasKey(product => product.Name);
        }
    }
}
