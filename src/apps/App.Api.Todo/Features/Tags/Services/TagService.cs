using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Features.Tags.Mapper;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;

namespace App.Api.Todo.Features.Tags.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly ITagMapper _tagMapper;
        private readonly ILogger<TagService> _logger;

        public TagService(ITagRepository tagRepository, ITagMapper tagMapper, ILogger<TagService> logger)
        {
            _tagRepository = tagRepository;
            _tagMapper = tagMapper;
            _logger = logger;
        }

        public async Task<Result<TagDto, ApiError>> CreateAsync(TagDto dto)
        {
            var tag = _tagMapper.ToEntity(dto);
            await _tagRepository.CreateAsync(tag);
            return _tagMapper.ToDto(tag);
        }

        public async Task<Result<bool, ApiError>> DeleteAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag is null) return false;

            await _tagRepository.DeleteAsync(tag);
            return true;
        }

        public async Task<Result<IEnumerable<TagDto>, ApiError>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all tags from the repository.");
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(_tagMapper.ToDto).ToList();
        }

        public async Task<Result<TagDto?, ApiError>> GetByIdAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            return tag is null ? null : _tagMapper.ToDto(tag);
        }

        public async Task<Result<bool, ApiError>> UpdateAsync(int id, TagDto dto)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            tag.Name = dto.Name;
            await _tagRepository.UpdateAsync(tag);
            return true;
        }
    }
}
