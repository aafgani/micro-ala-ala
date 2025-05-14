using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Features.Tags.Dtos;
using App.Api.Todo.Features.Tags.Mapper;

namespace App.Api.Todo.Features.Tags.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly ITagMapper _tagMapper;

        public TagService(ITagRepository tagRepository, ITagMapper tagMapper)
        {
            _tagRepository = tagRepository;
            _tagMapper = tagMapper;
        }

        public async Task<TagDto> CreateAsync(TagDto dto)
        {
            var tag = _tagMapper.ToEntity(dto);
            await _tagRepository.CreateAsync(tag);
            return _tagMapper.ToDto(tag);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag is null) return false;

            await _tagRepository.DeleteAsync(tag);
            return true;
        }

        public async Task<IEnumerable<TagDto>> GetAllAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(_tagMapper.ToDto);
        }

        public async Task<TagDto?> GetByIdAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            return tag is null ? null : _tagMapper.ToDto(tag);
        }

        public async Task<bool> UpdateAsync(int id, TagDto dto)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            tag.Name = dto.Name;
            await _tagRepository.UpdateAsync(tag);  
            return true;
        }
    }
}
