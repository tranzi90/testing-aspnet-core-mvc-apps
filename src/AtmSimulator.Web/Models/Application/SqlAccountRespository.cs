using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Application
{
    public class SqlAccountRespository : IAccountRepository
    {
        public Maybe<Account> Get(CustomerName customerName)
            => Maybe<Account>.None;

        public Maybe<Account> Get(PaymentCardNumber paymentCardNumber)
            => Maybe<Account>.None;

        public Result Register(Account account)
            => Result.Success();

        public Result Update(Account account)
            => Result.Success();
    }
}
