using ReferenceWebApi.Application.Dtos;

namespace ReferenceWebApi.Application.Interfaces
{
    public interface IServiceBase<TDto, TCreateDto, TUpdateDto>
    {
        Task<TDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PagedResultDto<TDto>> GetPagedAsync(
            PaginationParametersDto parameters,
            CancellationToken cancellationToken = default);
        Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default);
        Task<TDto> UpdateAsync(int id, TUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
