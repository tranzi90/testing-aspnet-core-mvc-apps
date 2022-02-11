using System;
using AtmSimulator.Web.Database;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Application
{
    public class SqlCustomerRepository : ICustomerRepository
    {
        private readonly AtmSimulatorDbContext _atmSimulatorDbContext;

        public SqlCustomerRepository(AtmSimulatorDbContext atmSimulatorDbContext)
        {
            _atmSimulatorDbContext = atmSimulatorDbContext;
        }

        public Maybe<Customer> Get(CustomerName name)
        {
            var dal = _atmSimulatorDbContext.Customers.Find(name.Name);

            if (dal is null)
            {
                return Maybe<Customer>.None;
            }

            return dal.ToDomain();
        }

        public Result Register(Customer customer)
        {
            try
            {
                var dal = customer.ToDal();

                _atmSimulatorDbContext.Customers.Add(dal);

                _atmSimulatorDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }

            return Result.Success();
        }

        public Result Update(Customer customer)
        {
            try
            {
                var dal = _atmSimulatorDbContext.Customers.Find(customer.Name.Name);

                if (dal is null)
                {
                    return Result.Failure("Can't find corresponding Atm to update.");
                }

                dal.Cash = customer.Cash;
                dal.AccountId = customer.AccountId;

                _atmSimulatorDbContext.Customers.Update(dal);

                _atmSimulatorDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }

            return Result.Success();
        }
    }
}
