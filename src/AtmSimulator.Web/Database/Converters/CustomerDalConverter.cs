using AtmSimulator.Web.Models.Domain;

namespace AtmSimulator.Web.Database
{
    public static class CustomerDalConverter
    {
        public static CustomerDal ToDal(this Customer customer)
            => new CustomerDal
            {
                Name = customer.Name.Name,
                Cash = customer.Cash,
                AccountId = customer.AccountId,
            };

        public static Customer ToDomain(this CustomerDal dal)
            => Customer.Create(
                CustomerName.Create(dal.Name),
                dal.Cash,
                dal.AccountId);
    }
}
