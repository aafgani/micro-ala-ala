using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Controllers
{
    public class TodoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
