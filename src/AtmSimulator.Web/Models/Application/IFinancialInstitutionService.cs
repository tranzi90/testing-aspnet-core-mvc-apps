using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Application
{
    public interface IFinancialInstitutionService
    {
        Result<Customer> RegisterCustomer(CustomerName customerName, decimal cash);

        Result<Atm> RegisterAtm(decimal balance);

        Result DeletePaymentCard(PaymentCardNumber paymentCardNumber);

        Result<PaymentCard> IssueNewPaymentCard(CustomerName customerName);
    }
}