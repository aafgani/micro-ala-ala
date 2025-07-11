$(document).ready(function () {
  $.get(
    "/Component/GetMoneyComponent?argument=MonthlyExpense",
    function (html) {
      $("#MonthlyExpense").html(html);
    }
  );
  $.get("/Component/GetMoneyComponent?argument=MonthlyIncome", function (html) {
    $("#MonthlyIncome").html(html);
  });
  $.get("/Component/GetMoneyComponent?argument=TotalBalance", function (html) {
    $("#TotalBalance").html(html);
  });
  $.get("/Component/GetMoneyComponent?argument=SavingsRate", function (html) {
    $("#SavingsRate").html(html);
  });
  $.get(
    "/Component/GetMoneyComponent?argument=WeatherForecast",
    function (html) {
      $("#WeatherForecast").html(html);
    }
  );
});
