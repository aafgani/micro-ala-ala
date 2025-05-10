using App.Web.Client.Models.ViewComponents;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Utilities.CustomViewComponent
{
    public class InfoBoxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(InfoBoxModel model)
        {
            return View(model);
        }
    }
}
