using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using App.Common.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Todo.Features.Todotask.Data
{
    public class TodoTaskRepository : Repository<Models.TodoTask>, ITodoTaskRepository
    {
        public TodoTaskRepository(TodoContext db) : base(db)
        {
        }

        public Task<TodoTask?> GetByIdWithRelationsAsync(int id)
        {
            return Set
                .Include(t => t.InverseParentTask)
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<(IEnumerable<Models.TodoTask>, int, int)> GetWithParamAsync(TodoTaskQueryParam param)
        {
            var query = Set.AsQueryable();

            if (param.DueDate.HasValue)
            {
                query = query.Where(c => c.DueDate == param.DueDate);
            }

            if (!string.IsNullOrEmpty(param.SortBy))
            {
                query = param.SortDirection?.ToLower() switch
                {
                    "desc" => param.SortBy.ToLower() switch
                    {
                        "title" => query.OrderByDescending(c => c.Title),
                        "duedate" => query.OrderByDescending(c => c.DueDate),
                        _ => query.OrderByDescending(c => c.Title)
                    },
                    "asc" => param.SortBy.ToLower() switch
                    {
                        "title" => query.OrderBy(c => c.Title),
                        "duedate" => query.OrderBy(c => c.DueDate),
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
