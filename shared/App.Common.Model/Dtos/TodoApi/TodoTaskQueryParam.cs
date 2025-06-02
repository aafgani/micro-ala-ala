using System.ComponentModel;

namespace App.Common.Domain.Dtos.TodoApi
{
    public enum SortDirection
    {
        Asc,
        Desc
    }

    public class TodoTaskQueryParam
    {
        public string? SortBy { get; set; }
        public SortDirection? SortDirection { get; set; }

        public bool? IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }

        [DefaultValue(1)]
        public int Page { get; set; }

        [DefaultValue(10)]
        public int PageSize { get; set; }

        public void ApplyDefaults()
        {
            SortBy ??= "Title";
            SortDirection ??= TodoApi.SortDirection.Asc;
            Page = Page <= 0 ? 1 : Page;
            PageSize = PageSize <= 0 ? 10 : Math.Min(PageSize, 100); // Also cap max page size
        }
    }
}
