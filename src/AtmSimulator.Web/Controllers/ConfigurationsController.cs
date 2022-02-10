using AtmSimulator.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtmSimulator.Web.Controllers
{
    [Route("api/v1/configurations")]
    public class ConfigurationsController : Controller
    {
        private readonly SampleOptions _sampleOptions;

        public ConfigurationsController(IOptions<SampleOptions> sampleOptions)
        {
            _sampleOptions = sampleOptions.Value;
        }

        [HttpGet("sample")]
        public ActionResult GetSample()
            => Ok(_sampleOptions);
    }
}
