using System;
using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AtmSimulator.Web.Controllers
{
    [Route("api/v1/transfers")]
    public class TransfersController : BaseController
    {
        private readonly IFinancialTransferSystemService _financialTransferSystem;
        public TransfersController(IFinancialTransferSystemService financialTransferSystem)
        {
            _financialTransferSystem = financialTransferSystem;
        }

        [HttpPost("payment-cards/{paymentCardNumber}/atms/{atmId:guid}")]
        public ActionResult DepositToAtm(
            [FromRoute] string paymentCardNumber,
            [FromRoute] Guid atmId,
            [FromQuery] decimal amount)
        {
            var paymentCardNumberDomain = PaymentCardNumber.TryParse(paymentCardNumber);

            if (paymentCardNumberDomain.IsFailure)
            {
                return BadRequest(paymentCardNumberDomain.Error);
            }

            var withdrawResult = _financialTransferSystem.DepositToAtm(
                paymentCardNumberDomain.Value,
                atmId,
                amount);

            return OkUnprocessableResult(withdrawResult);
        }

        [HttpPost("payment-cards/{senderPaymentCardNumber}/payment-cards/{recipientPaymentCardNumber}")]
        public ActionResult TransferToAnotherCustomer(
            [FromRoute] string senderPaymentCardNumber,
            [FromRoute] string recipientPaymentCardNumber,
            [FromQuery] decimal amount)
        {
            var senderPaymentCardNumberDomain = PaymentCardNumber.TryParse(senderPaymentCardNumber);

            if (senderPaymentCardNumberDomain.IsFailure)
            {
                return BadRequest(senderPaymentCardNumberDomain.Error);
            }

            var recipientPaymentCardNumberDomain = PaymentCardNumber.TryParse(recipientPaymentCardNumber);

            if (recipientPaymentCardNumberDomain.IsFailure)
            {
                return BadRequest(recipientPaymentCardNumberDomain.Error);
            }

            var transferResult = _financialTransferSystem.TransferToAnotherCustomer(
                senderPaymentCardNumberDomain.Value,
                recipientPaymentCardNumberDomain.Value,
                amount);

            return OkUnprocessableResult(transferResult);
        }

        [HttpPost("atms/{atmId:guid}/payment-cards/{paymentCardNumber}")]
        public ActionResult WithdrawFromAtm(
            [FromRoute] Guid atmId,
            [FromRoute] string paymentCardNumber,
            [FromQuery] decimal amount)
        {
            var paymentCardNumberDomain = PaymentCardNumber.TryParse(paymentCardNumber);

            if (paymentCardNumberDomain.IsFailure)
            {
                return BadRequest(paymentCardNumberDomain.Error);
            }

            var withdrawResult = _financialTransferSystem.WithdrawFromAtm(
                paymentCardNumberDomain.Value,
                atmId,
                amount);

            return OkUnprocessableResult(withdrawResult);
        }
    }
}
