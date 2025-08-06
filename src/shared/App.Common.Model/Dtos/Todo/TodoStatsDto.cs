namespace App.Common.Domain.Dtos.Todo;

public class TodoStatsDto
{
    public int TotalTodos { get; set; }
    public int CompletedTodos { get; set; }
    public int PendingTodos { get; set; }
    public double CompletionRate { get; set; }
    public int? TodaysCreated { get; set; }
    public int? TodaysCompleted { get; set; }
}
