using App.Api.Todo.Features.Tags.Dtos;
using App.Api.Todo.Models;

namespace App.Api.Todo.Features.Tags.Mapper
{
    public interface ITagMapper
    {
        TagDto ToDto(Tag tag);
        Tag ToEntity(TagDto tagDto);
    }
}
