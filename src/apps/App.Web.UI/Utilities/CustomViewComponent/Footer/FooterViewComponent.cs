using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Utilities.CustomViewComponent.Footer;

public class FooterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var version = Assembly.GetExecutingAssembly()
                                 .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                         ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                         ?? "1.0.0";
        return View("Default", version);
    }
}
