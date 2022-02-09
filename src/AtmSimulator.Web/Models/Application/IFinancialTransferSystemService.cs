using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Application
{
    public interface IFinancialTransferSystemService
    {
        Result DepositToAtm(
            PaymentCardNumber paymentCardNumber,
            Guid atmId,
            decimal amount);


        Result TransferToAnotherCustomer(
            PaymentCardNumber senderPaymentCardNumber,
            PaymentCardNumber recipientPaymentCardNumber,
            decimal amount);

        Result WithdrawFromAtm(
            PaymentCardNumber paymentCardNumber,
            Guid atmId,
            decimal amount);
    }
}