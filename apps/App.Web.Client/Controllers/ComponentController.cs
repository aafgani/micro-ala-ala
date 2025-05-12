using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Controllers
{
    [Route("component/[action]")]
    public class ComponentController : Controller
    {
        public async Task<IActionResult> GetMoneyComponent(string argument)
        {
            return ViewComponent("InfoBox", argument);
        }
    }
}
    