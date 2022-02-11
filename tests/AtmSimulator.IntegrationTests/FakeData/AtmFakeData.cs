using System;
using AtmSimulator.Web.Models.Domain;
using Bogus;

namespace AtmSimulator.IntegrationTests
{
    public class AtmFakeData
    {
        public AtmFakeData(int seed)
        {
            Valid = Valid.UseSeed(seed);
        }

        public Faker<Atm> Valid { get; private set; }
            = new Faker<Atm>("uk")
                .CustomInstantiator(f => Atm.Create(f.Random.Guid(), Math.Round(f.Random.Decimal(1, 1000), 2)));
    }
}
