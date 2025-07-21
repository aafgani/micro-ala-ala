using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;

namespace App.Api.Todo.Features.Tags.Services
{
    public interface ITagService
    {
        Task<Result<IEnumerable<TagDto>, ApiError>> GetAllAsync();
        Task<Result<TagDto?, ApiError>> GetByIdAsync(int id);
        Task<Result<TagDto, ApiError>> CreateAsync(TagDto dto);
        Task<Result<bool, ApiError>> UpdateAsync(int id, TagDto dto);
        Task<Result<bool, ApiError>> DeleteAsync(int id);
    }
}
