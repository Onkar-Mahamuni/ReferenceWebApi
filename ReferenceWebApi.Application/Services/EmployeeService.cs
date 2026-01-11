using AutoMapper;
using ReferenceWebApi.Application.Dtos.Employee;
using ReferenceWebApi.Application.Exceptions;
using ReferenceWebApi.Application.Interfaces;
using ReferenceWebApi.Domain.Entities;
using ReferenceWebApi.Domain.Interfaces;

namespace ReferenceWebApi.Application.Services
{
    public class EmployeeService : ServiceBase<Employee, EmployeeDto, CreateEmployeeDto, UpdateEmployeeDto>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        //private readonly ICacheService _cacheService;
        //private const string CacheKeyPrefix = "Employee";

        public EmployeeService(IEmployeeRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _employeeRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public override async Task<EmployeeDto> CreateAsync(
            CreateEmployeeDto createDto,
            CancellationToken cancellationToken = default)
        {
            // Business validation
            if (await _employeeRepository.EmailExistsAsync(createDto.Email, cancellationToken: cancellationToken))
            {
                throw new BusinessException($"Employee with email '{createDto.Email}' already exists");
            }

            return await base.CreateAsync(createDto, cancellationToken);
        }

        public override async Task<EmployeeDto> UpdateAsync(
            int id,
            UpdateEmployeeDto updateDto,
            CancellationToken cancellationToken = default)
        {
            // Business validation
            if (await _employeeRepository.EmailExistsAsync(updateDto.Email, id, cancellationToken))
            {
                throw new BusinessException($"Employee with email '{updateDto.Email}' already exists");
            }

            return await base.UpdateAsync(id, updateDto, cancellationToken);
        }

        public async Task<EmployeeDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email, cancellationToken);
            return employee is not null ? _mapper.Map<EmployeeDto>(employee) : null;
        }

        public async Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(
            string department,
            CancellationToken cancellationToken = default)
        {
            var employees = await _employeeRepository.GetByDepartmentAsync(department, cancellationToken);
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }
    }
}
