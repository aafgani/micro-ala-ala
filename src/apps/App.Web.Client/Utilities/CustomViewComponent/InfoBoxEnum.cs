﻿namespace App.Web.Client.Utilities.CustomViewComponent
{
    public enum InfoBoxEnum
    {
        TotalBalance,
        MonthlyIncome,
        MonthlyExpense,
        SavingsRate,
        WeatherForecast
    }

    public static class InfoBoxEnumExtensions
    {
        public static string GetDisplayName(this InfoBoxEnum value)
        {
            return value switch
            {
                InfoBoxEnum.TotalBalance => "Total Balance",
                InfoBoxEnum.MonthlyIncome => "MonthlyIncome",
                InfoBoxEnum.MonthlyExpense => "Monthly Expense",
                InfoBoxEnum.SavingsRate => "Savings Rate",
                InfoBoxEnum.WeatherForecast => "Weather Forecast",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }
}
