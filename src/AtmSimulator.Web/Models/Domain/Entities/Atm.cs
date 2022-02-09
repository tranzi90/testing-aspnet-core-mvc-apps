using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Dawn;

namespace AtmSimulator.Web.Models.Domain
{
    public sealed class Atm : ValueObject
    {
        private Atm(
            Guid id,
            decimal balance)
        {
            Id = Guard.Argument(id, nameof(id)).NotDefault();
            Balance = Guard.Argument(balance, nameof(balance)).NotNegative();
        }

        public Guid Id { get; }

        public decimal Balance { get; private set; }

        public static Atm Create(
            Guid id,
            decimal balance)
            => new Atm(id, balance);

        public static Result<Atm> TryCreate(
            Guid id,
            decimal balance)
            => Validate(id, balance)
            .Map(() => Create(id, balance));

        public static Result Validate(
            Guid id,
            decimal balance)
            => Result.Success()
            .Ensure(() => id != Guid.Empty, "ATM's id MUST not be default.")
            .Ensure(() => balance >= 0, "ATM's balance MUST be greater than or equal to zero.");

        private bool CanWithdraw(decimal amount)
            => Balance - amount >= 0;

        public Result Withdraw(decimal amount)
            => Result.SuccessIf(CanWithdraw(amount), "ATM has not enough money.")
            .Tap(() => Balance -= amount);

        public void Deposit(decimal amount)
            => Balance += amount;

        public override string ToString()
            => $"🏧: Id=[{Id}]. Balance=[{Balance.ToString("C2")}].";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
        }
    }
}
