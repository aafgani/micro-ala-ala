namespace App.Common.Domain.Dtos.Todo
{
    public class TodoTaskQueryParam : QueryParam
    {
        public bool? IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public void ApplyDefaults()
        {
            SortBy ??= "Title";
            SortDirection ??= "asc";
            Page = Page == 0 ? 1 : Page;
            PageSize = PageSize == 0 ? 10 : PageSize;
        }
    }
}
