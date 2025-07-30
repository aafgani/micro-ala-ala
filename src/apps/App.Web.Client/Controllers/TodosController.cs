using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Controllers
{
    [AllowAnonymous]
    public class TodosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
