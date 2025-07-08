using App.Api.Todo.Models;
using App.Common.Domain.Dtos;

namespace App.Api.Todo.Features.Tags.Mapper
{
    public interface ITagMapper
    {
        TagDto ToDto(Tag tag);
        Tag ToEntity(TagDto tagDto);
    }
}
