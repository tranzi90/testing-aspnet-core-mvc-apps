using System;
using AtmSimulator.Web.Models.Domain;
using Bogus;

namespace AtmSimulator.IntegrationTests
{
    public class CustomerFakeData
    {
        private readonly int _seed;
        private readonly CustomerNameFakeData _customerNameFakeData;

        public CustomerFakeData(int seed, CustomerNameFakeData customerNameFakeData)
        {
            _seed = seed;
            _customerNameFakeData = customerNameFakeData;
        }

        public Faker<Customer> Valid()
            => new Faker<Customer>("uk")
                .UseSeed(_seed)
                .CustomInstantiator(f => Customer.Create(
                    _customerNameFakeData.Valid.Generate(),
                    Math.Round(f.Random.Decimal(1, 1000), 2),
                    f.Random.Guid()));
    }
}
