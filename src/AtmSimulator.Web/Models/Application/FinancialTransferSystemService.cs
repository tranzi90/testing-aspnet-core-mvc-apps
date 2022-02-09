using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Application
{
    public sealed class FinancialTransferSystemService : IFinancialTransferSystemService
    {
        private readonly TransferService _paymentDomainService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAtmRepository _atmRepository;

        public FinancialTransferSystemService(
            TransferService paymentDomainService,
            ICustomerRepository customerRepository,
            IAccountRepository accountRepository,
            IAtmRepository atmRepository)
        {
            _paymentDomainService = paymentDomainService;
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _atmRepository = atmRepository;
        }

        public Result DepositToAtm(
            PaymentCardNumber paymentCardNumber,
            Guid atmId,
            decimal amount)
        {
            var maybeAccount = _accountRepository.Get(paymentCardNumber);

            if (maybeAccount.HasNoValue)
            {
                return Result.Failure("Can't find account by provided payment card number.");
            }

            var account = maybeAccount.Value;

            var maybeCustomer = _customerRepository.Get(account.CustomerName);

            if (maybeCustomer.HasNoValue)
            {
                return Result.Failure("Can't find customer by provided customer name.");
            }

            var maybeAtm = _atmRepository.Get(atmId);

            if (maybeAtm.HasNoValue)
            {
                return Result.Failure("Can't find ATM with provided id.");
            }

            var customer = maybeCustomer.Value;
            var atm = maybeAtm.Value;

            var depositResult = _paymentDomainService
                .DepositToAtm(customer, account, atm, amount)
                .Bind(() => _customerRepository.Update(customer))
                .Bind(() => _accountRepository.Update(account))
                .Bind(() => _atmRepository.Update(atm));

            return depositResult;
        }

        public Result WithdrawFromAtm(
            PaymentCardNumber paymentCardNumber,
            Guid atmId,
            decimal amount)
        {
            var maybeAccount = _accountRepository.Get(paymentCardNumber);

            if (maybeAccount.HasNoValue)
            {
                return Result.Failure("Can't find account by provided payment card number.");
            }

            var account = maybeAccount.Value;

            var maybeCustomer = _customerRepository.Get(account.CustomerName);

            if (maybeCustomer.HasNoValue)
            {
                return Result.Failure("Can't find customer by provided customer name.");
            }

            var maybeAtm = _atmRepository.Get(atmId);

            if (maybeAtm.HasNoValue)
            {
                return Result.Failure("Can't find ATM with provided id.");
            }

            var customer = maybeCustomer.Value;
            var atm = maybeAtm.Value;

            var withdrawResult = _paymentDomainService
                .WithdrawFromAtm(customer, account, atm, amount)
                .Bind(() => _customerRepository.Update(customer))
                .Bind(() => _accountRepository.Update(account))
                .Bind(() => _atmRepository.Update(atm));

            return withdrawResult;
        }

        public Result TransferToAnotherCustomer(
            PaymentCardNumber senderPaymentCardNumber,
            PaymentCardNumber recipientPaymentCardNumber,
            decimal amount)
        {
            var maybeSender = _accountRepository.Get(senderPaymentCardNumber);

            if (maybeSender.HasNoValue)
            {
                return Result.Failure("Can't find sender by provided payment card number.");
            }

            var maybeRecipient = _accountRepository.Get(recipientPaymentCardNumber);

            if (maybeRecipient.HasNoValue)
            {
                return Result.Failure("Can't find recipient by provided payment card number.");
            }

            var sender = maybeSender.Value;
            var recipient = maybeRecipient.Value;

            var transferResult = _paymentDomainService
                .TransferToAnotherAccount(sender, recipient, amount)
                .Bind(() => _accountRepository.Update(sender))
                .Bind(() => _accountRepository.Update(recipient));

            return transferResult;
        }
    }
}
