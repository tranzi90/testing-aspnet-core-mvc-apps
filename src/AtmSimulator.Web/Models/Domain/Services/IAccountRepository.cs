using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Domain
{
    public interface IAccountRepository
    {
        Result Register(Account account);

        Maybe<Account> Get(CustomerName customerName);

        Maybe<Account> Get(PaymentCardNumber paymentCardNumber);

        Result Update(Account account);
    }
}
