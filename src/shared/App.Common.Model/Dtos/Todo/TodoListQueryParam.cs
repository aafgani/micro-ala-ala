using System;

namespace App.Common.Domain.Dtos.Todo;

public class TodoListQueryParam : QueryParam
{
    public string? UserId { get; set; }
    public string? Title { get; set; }
    public void ApplyDefaults()
    {
        SortBy ??= "Title";
        SortDirection ??= "asc";
        Page = Page == 0 ? 1 : Page;
        PageSize = PageSize == 0 ? 10 : PageSize;
    }
}
