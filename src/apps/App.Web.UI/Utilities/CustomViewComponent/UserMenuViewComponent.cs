using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Utilities.CustomViewComponent;

public class UserMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        // You can pass any model data to the view here
        return View();
    }
}
