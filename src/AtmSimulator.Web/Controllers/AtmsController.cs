using System;
using AtmSimulator.Web.Dtos;
using AtmSimulator.Web.Models.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtmSimulator.Web.Controllers
{
    [Route("api/v1/atms")]
    public class AtmsController : BaseController
    {
        private readonly IFinancialInformationService _financialInformation;
        private readonly IFinancialInstitutionService _financialInstituion;

        public AtmsController(
            IFinancialInformationService financialInformation,
            IFinancialInstitutionService financialInstituion)
        {
            _financialInformation = financialInformation;
            _financialInstituion = financialInstituion;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<RegisteredAtmResponseDto> RegisterAtm(
            [FromQuery] decimal balance)
        {
            var registeredAtm = _financialInstituion.RegisterAtm(balance);

            return CreatedUnprocessableResult(registeredAtm, x => x.ToDto());
        }

        [HttpGet("{atmId:guid}/balance")]
        public ActionResult<decimal> CheckBalance(
            [FromRoute] Guid atmId)
        {
            var atmBalance = _financialInformation.CheckAtmBalance(atmId);

            return OkUnprocessableResult(atmBalance, x => x);
        }
    }
}
