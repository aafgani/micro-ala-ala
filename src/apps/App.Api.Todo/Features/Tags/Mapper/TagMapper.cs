using App.Api.Todo.Features.Tags.Dtos;
using App.Api.Todo.Models;
using Riok.Mapperly.Abstractions;

namespace App.Api.Todo.Features.Tags.Mapper
{
    [Mapper]
    public partial class TagMapper : ITagMapper
    {
        public partial TagDto ToDto(Tag tag);

        public partial Tag ToEntity(TagDto tagDto);
    }
}
