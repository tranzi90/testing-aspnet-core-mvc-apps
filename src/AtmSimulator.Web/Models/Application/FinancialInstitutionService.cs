using System.Linq;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Application
{
    public sealed class FinancialInstitutionService : IFinancialInstitutionService
    {
        private readonly PaymentCardGenerator _paymentCardGenerator;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAtmRepository _atmRepository;
        private readonly IRandomGenerator _randomGenerator;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FinancialInstitutionService(
            PaymentCardGenerator paymentCardGenerator,
            ICustomerRepository customerRepository,
            IAccountRepository accountRepository,
            IAtmRepository atmRepository,
            IRandomGenerator randomGenerator,
            IDateTimeProvider dateTimeProvider)
        {
            _paymentCardGenerator = paymentCardGenerator;
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _atmRepository = atmRepository;
            _randomGenerator = randomGenerator;
            _dateTimeProvider = dateTimeProvider;
        }

        public Result<Customer> RegisterCustomer(CustomerName customerName, decimal cash)
        {
            var maybeExistingCustomer = _customerRepository.Get(customerName);

            if (maybeExistingCustomer.HasValue)
            {
                return Result.Failure<Customer>("Customer is already registered.");
            }

            var accountId = _randomGenerator.NewGuid();
            var account = Account.CreateForCustomer(accountId, customerName);

            return from customer in Customer.TryCreate(customerName, cash, accountId)
                   from customerRegistration in _customerRepository.Register(customer).Map(() => customer)
                   from accountRegistration in _accountRepository.Register(account).Map(() => customer)
                   select customer;
        }

        public Result<Atm> RegisterAtm(decimal balance)
        {
            var atmId = _randomGenerator.NewGuid();

            return from atm in Atm.TryCreate(atmId, balance)
                   from atmRegistration in _atmRepository.Register(atm).Map(() => atm)
                   select atm;
        }

        public Result<PaymentCard> IssueNewPaymentCard(CustomerName customerName)
        {
            var now = _dateTimeProvider.UtcNow;

            var maybeAccount = _accountRepository.Get(customerName);

            if (maybeAccount.HasNoValue)
            {
                return Result.Failure<PaymentCard>("Can't find account by provided customer name.");
            }

            var account = maybeAccount.Value;

            var newPaymentCard = _paymentCardGenerator.GenerateNewCard(
                account.CustomerName,
                now);

            var bindResult = account.BindCard(newPaymentCard, now);

            return bindResult
                .Bind(() => _accountRepository.Update(account))
                .Map(() => newPaymentCard);
        }

        public Result DeletePaymentCard(PaymentCardNumber paymentCardNumber)
        {
            var maybeAccount = _accountRepository.Get(paymentCardNumber);

            if (maybeAccount.HasNoValue)
            {
                return Result.Failure("Can't find account by provided payment card number.");
            }

            var account = maybeAccount.Value;

            var deleteCardResult = account.DeleteCard(paymentCardNumber)
                .Bind(() => _accountRepository.Update(account));

            return deleteCardResult;
        }
    }
}
