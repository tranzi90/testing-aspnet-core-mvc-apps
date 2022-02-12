using System.Collections.Generic;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.FunctionalTests.Bdd.Fakes
{
    public class FakeCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = new List<Customer>(0);

        public Maybe<Customer> Get(CustomerName name)
            => _customers.Find(x => x.Name == name) ?? Maybe<Customer>.None;

        public Result Register(Customer customer)
            => Result.Success()
            .Tap(() => _customers.Add(customer));

        public Result Update(Customer customer)
            => Result.SuccessIf(_customers.Contains(customer), "Customer was not found.")
            .Tap(() =>
            {
                var index = _customers.IndexOf(customer);

                _customers[index] = customer;
            });
    }
}
