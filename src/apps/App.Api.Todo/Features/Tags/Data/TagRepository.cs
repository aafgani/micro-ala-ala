using App.Api.Todo.Models;
using App.Common.Infrastructure.Model;

namespace App.Api.Todo.Features.Tags.Data
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(TodoContext db) : base(db)
        {
        }
    }
}
