using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Finance()
        {
            return View();
        }

        public IActionResult Todo()
        {
            return View();
        }
    }
}
