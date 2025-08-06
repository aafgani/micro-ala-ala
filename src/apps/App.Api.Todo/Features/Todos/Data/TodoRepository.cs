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

    public async Task<(IEnumerable<MyTodo>, int, int)> GetWithParamAsync(TodoListQueryParam param)
    {
        var query = Set.AsQueryable();

        if (!string.IsNullOrEmpty(param.Title))
        {
            query = query.Where(c => c.Title == param.Title);
        }

        if (!string.IsNullOrEmpty(param.UserId))
        {
            query = query.Where(c => c.CreatedBy == param.UserId);
        }

        if (!string.IsNullOrEmpty(param.SortBy))
        {
            query = param.SortDirection?.ToLower() switch
            {
                "desc" => param.SortBy.ToLower() switch
                {
                    "title" => query.OrderByDescending(c => c.Title),
                    "userid" => query.OrderByDescending(c => c.AssignTo),
                    _ => query.OrderByDescending(c => c.Title)
                },
                "asc" => param.SortBy.ToLower() switch
                {
                    "title" => query.OrderBy(c => c.Title),
                    "userid" => query.OrderBy(c => c.AssignTo),
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
