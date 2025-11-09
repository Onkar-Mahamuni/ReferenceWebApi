using AutoMapper;
using ReferenceWebApi.Application.Dtos;
using ReferenceWebApi.Application.Exceptions;
using ReferenceWebApi.Application.Interfaces;
using ReferenceWebApi.Domain.Entities;
using ReferenceWebApi.Domain.Interfaces;

namespace ReferenceWebApi.Application.Services
{
    public abstract class ServiceBase<TEntity, TDto, TCreateDto, TUpdateDto>
        : IServiceBase<TDto, TCreateDto, TUpdateDto>
        where TEntity : EntityBase
    {
        protected readonly IRepositoryBase<TEntity> _repository;
        protected readonly IMapper _mapper;

        protected ServiceBase(IRepositoryBase<TEntity> repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public virtual async Task<TDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public virtual async Task<PagedResultDto<TDto>> GetPagedAsync(
            PaginationParametersDto parameters,
            CancellationToken cancellationToken = default)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(
                parameters.PageNumber,
                parameters.PageSize,
                cancellationToken);

            return new PagedResultDto<TDto>
            {
                Items = _mapper.Map<IEnumerable<TDto>>(items),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = totalCount
            };
        }

        public virtual async Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<TEntity>(createDto);
            var createdEntity = await _repository.AddAsync(entity, cancellationToken);
            return _mapper.Map<TDto>(createdEntity);
        }

        public virtual async Task<TDto> UpdateAsync(
            int id,
            TUpdateDto updateDto,
            CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            _mapper.Map(updateDto, entity);
            await _repository.UpdateAsync(entity, cancellationToken);

            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var exists = await _repository.ExistsAsync(id, cancellationToken);

            if (!exists)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            await _repository.DeleteAsync(id, cancellationToken);
        }
    }
}
