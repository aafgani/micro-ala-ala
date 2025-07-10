using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using App.Common.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Todo.Features.Todolist.Data
{
    public class TodolistRepository : Repository<ToDoList>, ITodolistRepository
    {
        public TodolistRepository(TodoContext db) : base(db)
        {
        }

        public async Task<(IEnumerable<ToDoList>, int, int)> GetWithParamAsync(TodoListQueryParam param)
        {
            var query = Set.AsQueryable();

            if (!string.IsNullOrEmpty(param.Title))
            {
                query = query.Where(c => c.Title == param.Title);
            }

            if (!string.IsNullOrEmpty(param.UserId))
            {
                query = query.Where(c => c.UserId == param.UserId);
            }

            if (!string.IsNullOrEmpty(param.SortBy))
            {
                query = param.SortDirection?.ToLower() switch
                {
                    "desc" => param.SortBy.ToLower() switch
                    {
                        "title" => query.OrderByDescending(c => c.Title),
                        "userid" => query.OrderByDescending(c => c.UserId),
                        _ => query.OrderByDescending(c => c.Title)
                    },
                    "asc" => param.SortBy.ToLower() switch
                    {
                        "title" => query.OrderBy(c => c.Title),
                        "userid" => query.OrderBy(c => c.UserId),
                        _ => query.OrderBy(c => c.Title)
                    }
                };
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)param.PageSize);

            var todoTasks = await query
                .Skip((param.Page - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            return (todoTasks, totalItems, totalPages);
        }
    }
}
