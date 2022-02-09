using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Application
{
    public sealed class FinancialInformationService : IFinancialInformationService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAtmRepository _atmRepository;

        public FinancialInformationService(
            ICustomerRepository customerRepository,
            IAccountRepository accountRepository,
            IAtmRepository atmRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _atmRepository = atmRepository;
        }

        public Result<decimal> CheckCustomerCash(CustomerName customerName)
            => _customerRepository
            .Get(customerName)
            .ToResult("Can't find customer by provided name.")
            .Map(customer => customer.Cash);

        public Result<decimal> CheckAccountBalance(PaymentCardNumber paymentCardNumber)
            => _accountRepository
            .Get(paymentCardNumber)
            .ToResult("Can't find account by provided payment card number.")
            .Map(account => account.Balance);

        public Result<decimal> CheckAtmBalance(Guid atmId)
            => _atmRepository
            .Get(atmId)
            .ToResult("Can't find ATM by provided id.")
            .Map(atm => atm.Balance);
    }
}
