using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Dawn;

namespace AtmSimulator.Web.Models.Domain
{
    public sealed class Customer : ValueObject
    {
        public CustomerName Name { get; }

        public decimal Cash { get; private set; }

        public Guid AccountId { get; }

        private Customer(
            CustomerName name,
            decimal cash,
            Guid accountId)
        {
            Name = Guard.Argument(name, nameof(name)).NotNull();
            Cash = Guard.Argument(cash, nameof(cash)).NotNegative();
            AccountId = Guard.Argument(accountId, nameof(accountId)).NotDefault();
        }

        public static Customer Create(
            CustomerName name,
            decimal cash,
            Guid accountId)
            => new Customer(name, cash, accountId);

        public static Result<Customer> TryCreate(
            CustomerName name,
            decimal cash,
            Guid accountId)
            => Validate(cash, accountId)
            .Map(() => new Customer(name, cash, accountId));

        public static Result Validate(
            decimal cash,
            Guid accountId)
            => Result.Success()
            .Ensure(() => cash >= 0, "Customer's cash MUST be greater than or equal to zero.")
            .Ensure(() => accountId != Guid.Empty, "Customer's account id MUST not be default");

        private bool CanWithdraw(decimal amount)
            => Cash - amount >= 0;

        public Result Withdraw(decimal amount)
            => Result.SuccessIf(CanWithdraw(amount), "Customer has not enough cash.")
            .Tap(() => Cash -= amount);

        public void Deposit(decimal amount)
            => Cash += amount;

        public override string ToString()
            => $"👤: Name=[{Name}]. Cash=[{Cash.ToString("C2")}].";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
