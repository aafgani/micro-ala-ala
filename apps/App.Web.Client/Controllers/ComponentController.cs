using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Controllers
{
    [Route("component/[action]")]
    public class ComponentController : Controller
    {
        public async Task<IActionResult> GetMoneyComponent(string argument)
        {
            var result = await Task.Run(() => 
            { 
                Thread.Sleep(1000); // Simulate a delay
                return ViewComponent("InfoBox", argument); 
            });
            return result;
        }
    }
}
    