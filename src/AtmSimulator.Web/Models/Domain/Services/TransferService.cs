using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Domain
{
    public sealed class TransferService
    {
        public Result DepositToAtm(
            Customer customer,
            Account account,
            Atm atm,
            decimal amount)
            => VerifyCustomersAccount(customer, account)
            .Bind(() => customer.Withdraw(amount))
            .Tap(() => account.Deposit(amount))
            .Tap(() => atm.Deposit(amount));

        public Result WithdrawFromAtm(
            Customer customer,
            Account account,
            Atm atm,
            decimal amount)
            => VerifyCustomersAccount(customer, account)
            .Bind(() => atm.Withdraw(amount))
            .Bind(() => account.Withdraw(amount))
            .Tap(() => customer.Deposit(amount));

        public Result TransferToAnotherAccount(
            Account sender,
            Account recipient,
            decimal amount)
            => sender.Withdraw(amount)
            .Tap(() => recipient.Deposit(amount));

        private static Result VerifyCustomersAccount(
            Customer customer,
            Account account)
            => Result.SuccessIf(customer.AccountId == account.Id, "Customer can manage only his account.");
    }
}
