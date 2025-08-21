using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using App.Common.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Todo.Features.Todos.Data;

public class TodoRepository : Repository<MyTodo>, ITodoRepository
{
    public TodoRepository(TodoContext db) : base(db)
    {
    }

    public async Task<TodoStatsDto> GetStatsAsync(string userId = null)
    {
        var query = Set.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(c => c.CreatedBy == userId);
        }

        var stats = await query
        .GroupBy(t => 1)
        .Select(g => new TodoStatsDto
        {
            TotalTodos = g.Count(),
            CompletedTodos = g.Count(c => c.IsCompleted),
            PendingTodos = g.Count(t => !t.IsCompleted),
            TodaysCreated = g.Count(t => t.CreatedAt.Date == DateTime.UtcNow.Date),
            TodaysCompleted = g.Count(t => t.IsCompleted && t.CreatedAt.Date == DateTime.UtcNow.Date)
        })
        .FirstOrDefaultAsync();

        if (stats == null)
        {
            return new TodoStatsDto
            {
                TotalTodos = 0,
                CompletedTodos = 0,
                PendingTodos = 0,
                CompletionRate = 0.0,
                TodaysCreated = 0,
                TodaysCompleted = 0
            };
        }

        stats.CompletionRate = stats.TotalTodos > 0
            ? Math.Round((double)stats.CompletedTodos / stats.TotalTodos * 100, 1)
            : 0.0;

        return stats;
    }

    public async Task<(IEnumerable<MyTodo>, int, int)> GetWithParamAsync(TodoListQueryParam param)
    {
        var query = Set.AsQueryable();

        if (!string.IsNullOrEmpty(param.Title))
        {
            query = query.Where(c => c.Title == param.Title);
        }

        if (!string.IsNullOrEmpty(param.CreatedBy))
        {
            query = query.Where(c => c.CreatedBy == param.CreatedBy);
        }

        if (!string.IsNullOrEmpty(param.SortBy))
        {
            query = param.SortDirection?.ToLower() switch
            {
                "desc" => param.SortBy.ToLower() switch
                {
                    "title" => query.OrderByDescending(c => c.Title),
                    "createdAt" => query.OrderByDescending(c => c.CreatedAt),
                    _ => query.OrderByDescending(c => c.Title)
                },
                "asc" => param.SortBy.ToLower() switch
                {
                    "title" => query.OrderBy(c => c.Title),
                    "createdAt" => query.OrderBy(c => c.CreatedAt),
                    _ => query.OrderBy(c => c.Title)
                }
            };
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)param.PageSize);

        var todos = await query
                 .Skip((param.Page - 1) * param.PageSize)
                 .Take(param.PageSize)
                 .ToListAsync();

        return (todos, totalItems, totalPages);
    }
}
