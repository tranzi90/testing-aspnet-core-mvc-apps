using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Domain
{
    public interface ICustomerRepository
    {
        Result Register(Customer customer);

        Maybe<Customer> Get(CustomerName name);

        Result Update(Customer customer);
    }
}
