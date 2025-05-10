using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Utilities.CustomViewComponent
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var version = Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0.0"; // Default to 1.0.0 if not set
            return View("Default", version);
        }
    }
}
