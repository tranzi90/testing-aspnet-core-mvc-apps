using System.Collections.Generic;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.FunctionalTests.Bdd.Fakes
{
    public class FakeAccountRepository : IAccountRepository
    {
        private readonly List<Account> _accounts = new List<Account>(0);

        public Maybe<Account> Get(CustomerName customerName)
            => _accounts.Find(account => account.CustomerName == customerName) ?? Maybe<Account>.None;

        public Maybe<Account> Get(PaymentCardNumber paymentCardNumber)
            => _accounts.Find(account => account.GetCard(paymentCardNumber).IsSuccess) ?? Maybe<Account>.None;

        public Result Register(Account account)
            => Result.Success()
            .Tap(() => _accounts.Add(account));

        public Result Update(Account account)
            => Result.SuccessIf(_accounts.Contains(account), "Account was not found.")
            .Tap(() =>
            {
                var index = _accounts.IndexOf(account);

                _accounts[index] = account;
            });
    }
}
