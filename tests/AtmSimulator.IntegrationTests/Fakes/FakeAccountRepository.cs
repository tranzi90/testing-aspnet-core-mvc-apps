using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.IntegrationTests.Fakes
{
    public class FakeAccountRepository : IAccountRepository
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
