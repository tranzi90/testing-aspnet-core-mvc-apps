using AtmSimulator.Web.Models.Domain;
using Bogus;

namespace AtmSimulator.UnitTests
{
    public class CustomerNameFakeData
    {
        public CustomerNameFakeData(int seed)
        {
            Valid = Valid.UseSeed(seed);
        }

        public Faker<CustomerName> Valid { get; private set; } 
            = new Faker<CustomerName>("uk")
                .CustomInstantiator(f => CustomerName.Create(f.Name.FullName()));
    }
}
