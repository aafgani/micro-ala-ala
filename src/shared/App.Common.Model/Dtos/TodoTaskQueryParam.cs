using System.ComponentModel;

namespace App.Common.Domain.Dtos
{
    public class TodoTaskQueryParam
    {
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }

        public bool? IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }

        [DefaultValue(1)]
        public int Page { get; set; }

        [DefaultValue(10)]
        public int PageSize { get; set; }

        public void ApplyDefaults()
        {
            SortBy ??= "Title";
            SortDirection ??= "asc";
            Page = Page == 0 ? 1 : Page;
            PageSize = PageSize == 0 ? 10 : PageSize;
        }
    }
}
