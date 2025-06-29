using App.Api.Todo.Features.Tags.Dtos;

namespace App.Api.Todo.Features.Tags.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllAsync();
        Task<TagDto?> GetByIdAsync(int id);
        Task<TagDto> CreateAsync(TagDto dto);
        Task<bool> UpdateAsync(int id, TagDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
