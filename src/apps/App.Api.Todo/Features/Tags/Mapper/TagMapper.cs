using App.Api.Todo.Models;
using App.Common.Domain.Dtos;
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
