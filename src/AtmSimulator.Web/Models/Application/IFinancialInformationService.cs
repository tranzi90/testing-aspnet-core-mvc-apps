using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Application
{
    public interface IFinancialInformationService
    {
        Result<decimal> CheckAccountBalance(PaymentCardNumber paymentCardNumber);

        Result<decimal> CheckAtmBalance(Guid atmId);

        Result<decimal> CheckCustomerCash(CustomerName customerName);
    }
}