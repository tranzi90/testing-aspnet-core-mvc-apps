using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtmSimulator.Web.Database
{
    public class AtmDalEntityConfiguration : IEntityTypeConfiguration<AtmDal>
    {
        private readonly string _schema;

        public AtmDalEntityConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<AtmDal> builder)
        {
            if (!string.IsNullOrWhiteSpace(_schema))
            {
                builder.ToTable(nameof(AtmSimulatorDbContext.Atms), _schema);
            }
            else
            {
                builder.ToTable(nameof(AtmSimulatorDbContext.Atms));
            }

            builder.HasKey(product => product.Id);
        }
    }
}
