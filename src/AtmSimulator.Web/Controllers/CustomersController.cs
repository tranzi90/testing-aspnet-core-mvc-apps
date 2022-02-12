using System.Linq;
using AtmSimulator.Web.Dtos;
using AtmSimulator.Web.Models.Application;
using AtmSimulator.Web.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtmSimulator.Web.Controllers
{
    [Route("api/v1/customers")]
    public class CustomersController : BaseController
    {
        private readonly IFinancialInformationService _financialInformation;
        private readonly IFinancialInstitutionService _financialInstituion;

        public CustomersController(
            IFinancialInformationService financialInformation,
            IFinancialInstitutionService financialInstituion)
        {
            _financialInformation = financialInformation;
            _financialInstituion = financialInstituion;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<RegisteredCustomerResponseDto> RegisterCustomer(
            [FromBody] RegisterCustomerRequestDto registerCustomerRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToArray();

                return BadRequest(errors);
            }

            var customerName = CustomerName.TryCreate(registerCustomerRequest.CustomerName);

            if (customerName.IsFailure)
            {
                return BadRequest(customerName.Error);
            }

            var registerCustomerResult = _financialInstituion.RegisterCustomer(
                customerName.Value,
                registerCustomerRequest.Cash);

            return CreatedUnprocessableResult(
                registerCustomerResult,
                d => d.ToDto());
        }

        [HttpGet("{customerName}/cash")]
        public ActionResult<decimal> GetCash(
            [FromRoute] string customerName)
        {
            var customerNameDomain = CustomerName.TryCreate(customerName);

            if (customerNameDomain.IsFailure)
            {
                return BadRequest(customerNameDomain.Error);
            }

            var customerCash = _financialInformation.CheckCustomerCash(customerNameDomain.Value);

            return OkUnprocessableResult(customerCash, x => x);
        }

        [HttpPost("{customerName}/payment-cards")]
        public ActionResult<IssuedNewPaymentCardResponseDto> IssueNewPaymentCard(
            [FromRoute] string customerName)
        {
            var customerNameDomain = CustomerName.TryCreate(customerName);

            if (customerNameDomain.IsFailure)
            {
                return BadRequest(customerNameDomain.Error);
            }

            var newPaymentCard = _financialInstituion.IssueNewPaymentCard(customerNameDomain.Value);

            return CreatedUnprocessableResult(newPaymentCard, x => x.ToDto());
        }
    }
}
