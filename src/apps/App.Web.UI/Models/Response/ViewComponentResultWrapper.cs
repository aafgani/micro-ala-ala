using App.Common.Domain.Dtos.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace App.Web.UI.Models.Response;

public sealed class ViewComponentResultWrapper<TValue, TError> : IViewComponentResult
{
    private readonly Result<TValue, TError> _result;
    private readonly string _successViewName;
    private readonly string _errorViewName;
    private readonly string _viewName;

    public ViewComponentResultWrapper(Result<TValue, TError> result, string viewName = "Default")
    {
        _result = result;
        _viewName = viewName;
    }

    public void Execute(ViewComponentContext context)
    {
        var viewResult = new ViewViewComponentResult
        {
            ViewName = _viewName,
            ViewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = _result.IsSuccess ? _result.Value : _result.Error
            }
        };
        viewResult.Execute(context);
    }

    public async Task ExecuteAsync(ViewComponentContext context)
    {
        var viewResult = new ViewViewComponentResult
        {
            ViewName = _result.IsSuccess ? _successViewName : _errorViewName,
            ViewData = new ViewDataDictionary(
                   new EmptyModelMetadataProvider(),
                   new ModelStateDictionary())
            {
                Model = _result.IsSuccess ? _result.Value : _result.Error
            }
        };

        await viewResult.ExecuteAsync(context);
    }

    public static implicit operator ViewComponentResultWrapper<TValue, TError>(Result<TValue, TError> result)
        => new(result);
}

