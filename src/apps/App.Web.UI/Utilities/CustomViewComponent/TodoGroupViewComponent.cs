using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Utilities.CustomViewComponent;

public class TodoGroupViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
