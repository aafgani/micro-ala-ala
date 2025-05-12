using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Utilities.CustomViewComponent
{
    public class InfoBoxLoadingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string arguments)
        {
            // Pass the arguments to the view if needed
            ViewData["Arguments"] = arguments;

            // Return the Default.cshtml view
            return View("Default");
        }
    }
}
