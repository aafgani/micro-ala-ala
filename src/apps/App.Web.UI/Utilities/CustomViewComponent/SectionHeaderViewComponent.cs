using App.Web.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Utilities.CustomViewComponent;

public class SectionHeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string title)
    {
        return View("Default", new SectionHeaderViewModel { Title = title });
    }
}
