using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AtmSimulator.Web.Controllers
{
    [Route("api/v1/accounts")]
    public class AccountsController : BaseController
    {
        private readonly IFinancialInformationService _financialInformation;
        private readonly IFinancialInstitutionService _financialInstituion;

        public AccountsController(
            IFinancialInformationService financialInformation,
            IFinancialInstitutionService financialInstituion)
        {
            _financialInformation = financialInformation;
            _financialInstituion = financialInstituion;
        }

        [HttpGet("balance")]
        public ActionResult<decimal> CheckBalance(
            [FromQuery] string paymentCardNumber)
        {
            var paymentCardNumberDomain = PaymentCardNumber.TryParse(paymentCardNumber);

            if (paymentCardNumberDomain.IsFailure)
            {
                return BadRequest(paymentCardNumberDomain.Error);
            }

            var accountBalance = _financialInformation.CheckAccountBalance(paymentCardNumberDomain.Value);

            return OkUnprocessableResult(accountBalance, x => x);
        }
    }
}
