using System.Diagnostics;
using System.Linq;
using AtmSimulator.Web.Models;
using AtmSimulator.Web.Models.Domain;
using AtmSimulator.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AtmSimulator.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAtmRepository _atmRepository;

        public HomeController(
            IAtmRepository atmRepository)
        {
            _atmRepository = atmRepository;
        }

        public ActionResult<HomeIndexViewModel> Index()
        {
            var atms = _atmRepository.GetAll();

            var atmViewModels = atms
                .Select(x => new AtmViewModel
                {
                    Id = x.Id,
                    Balance = x.Balance,
                })
                .ToArray();

            var viewModel = new HomeIndexViewModel
            {
                Atms = atmViewModels
            };

            ViewData["TotalCount"] = atmViewModels.Length;
            ViewBag.TotalCount = atmViewModels.Length;

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
