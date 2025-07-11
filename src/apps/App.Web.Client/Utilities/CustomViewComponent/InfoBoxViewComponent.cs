using App.Common.Domain.Dtos;
using App.Web.Client.Models.ViewComponents;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Utilities.CustomViewComponent
{
    public class InfoBoxViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<InfoBoxViewComponent> _logger;
        private readonly string _apiBaseUrl;

        public InfoBoxViewComponent(IHttpClientFactory httpClientFactory, ILogger<InfoBoxViewComponent> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiBaseUrl = configuration["TodoApiBaseUrl"] ?? "http://localhost:8081";
        }

        public async Task<IViewComponentResult> InvokeAsync(string title)
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
                case "WeatherForecast":
                    box = await GetWeatherForecastInfoAsync();
                    break;
                default:
                    break;
            }
            return View(box);
        }

        private async Task<InfoBoxModel> GetWeatherForecastInfoAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var weatherData = await client.GetFromJsonAsync<WeatherForecast[]>("" + _apiBaseUrl + "/weatherforecast");

                var todayWeather = weatherData?.FirstOrDefault();
                if (todayWeather != null)
                {
                    return new InfoBoxModel
                    {
                        Title = InfoBoxEnum.WeatherForecast.GetDisplayName(),
                        Value = $"{todayWeather.Summary}, {todayWeather.TemperatureC}°C",
                        IconClass = GetWeatherIcon(todayWeather.Summary),
                        BgColorClass = GetWeatherBgColor(todayWeather.Summary)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the weather forecast.");
            }

            // Fallback if API call fails
            return new InfoBoxModel
            {
                Title = InfoBoxEnum.WeatherForecast.GetDisplayName(),
                Value = "Weather unavailable",
                IconClass = "fas fa-cloud",
                BgColorClass = "bg-secondary"
            };
        }

        private string GetWeatherBgColor(string summary)
        {
            return summary?.ToLower() switch
            {
                "freezing" => "bg-info",         // Blue for freezing weather
                "bracing" => "bg-primary",       // Dark blue for bracing weather
                "chilly" => "bg-info",           // Blue for cold weather
                "cool" => "bg-primary",          // Blue for cool weather
                "mild" => "bg-secondary",        // Gray for mild weather
                "warm" => "bg-success",          // Green for nice warm weather
                "balmy" => "bg-warning",         // Orange/yellow for pleasant weather
                "hot" => "bg-warning",           // Orange for hot weather
                "sweltering" => "bg-danger",     // Red for extremely hot weather
                "scorching" => "bg-danger",      // Red for scorching weather
                _ => "bg-secondary"               // Gray for unknown weather
            };
        }

        private string GetWeatherIcon(string summary)
        {
            return summary?.ToLower() switch
            {
                "freezing" => "fas fa-icicles",
                "bracing" => "fas fa-snowflake",
                "chilly" => "fas fa-thermometer-quarter",
                "cool" => "fas fa-wind",
                "mild" => "fas fa-cloud-sun",
                "warm" => "fas fa-thermometer-half",
                "balmy" => "fas fa-sun",
                "hot" => "fas fa-thermometer-three-quarters",
                "sweltering" => "fas fa-fire",
                "scorching" => "fas fa-fire-flame-curved",
                _ => "fas fa-cloud-sun"
            };
        }
    }
}