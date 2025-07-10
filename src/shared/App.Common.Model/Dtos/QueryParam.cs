using System.ComponentModel;

namespace App.Common.Domain.Dtos;

public class QueryParam
{
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }

    [DefaultValue(1)]
    public int Page { get; set; }

    [DefaultValue(10)]
    public int PageSize { get; set; }

}
