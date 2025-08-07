using App.Web.Client.Models.ViewComponents;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Utilities.CustomViewComponent.Dashboard;

public class InformationBoxViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(InfoBoxModel model)
    {
        return View(model);
    }
}
