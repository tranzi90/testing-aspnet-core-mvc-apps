using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.IntegrationTests.Fakes
{
    public class FakeCustomerRepository : ICustomerRepository
    {
        public Maybe<Customer> Get(CustomerName name)
            => Maybe<Customer>.None;

        public Result Register(Customer customer)
            => Result.Success();

        public Result Update(Customer customer)
            => Result.Success();
    }
}
