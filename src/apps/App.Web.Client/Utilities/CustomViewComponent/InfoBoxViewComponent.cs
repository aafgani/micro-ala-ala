using App.Web.Client.Models.ViewComponents;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Utilities.CustomViewComponent
{
    public class InfoBoxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string title)
        {
            var box = new InfoBoxModel();
            switch (title)
            {
                case "TotalBalance":
                    box.Title = InfoBoxEnum.TotalBalance.GetDisplayName();
                    box.Value = "$ 1,000.00";
                    box.IconClass = "fas fa-wallet";
                    box.BgColorClass = "bg-info";
                    break;
                case "MonthlyIncome":
                    box.Title = InfoBoxEnum.MonthlyIncome.GetDisplayName();
                    box.Value = "$ 1,000.00";
                    box.IconClass = "fas fa-arrow-up";
                    box.BgColorClass = "bg-success";
                    break;
                case "MonthlyExpense":
                    box.Title = InfoBoxEnum.MonthlyExpense.GetDisplayName();
                    box.Value = "$ 1,000.00";
                    box.IconClass = "fas fa-arrow-down";
                    box.BgColorClass = "bg-danger";
                    break;
                case "SavingsRate":
                    box.Title = InfoBoxEnum.SavingsRate.GetDisplayName();
                    box.Value = "10%";
                    box.IconClass = "fas fa-piggy-bank";
                    box.BgColorClass = "bg-warning";
                    break;
                default:
                    break;
            }
            return View(box);
        }
    }
}
