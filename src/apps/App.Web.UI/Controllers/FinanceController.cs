using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Controllers
{

    public class FinanceController : Controller
    {
        private readonly ILogger<FinanceController> _logger;

        public FinanceController(ILogger<FinanceController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Navigated to Finance Index");
            return View();
        }
    }
}
