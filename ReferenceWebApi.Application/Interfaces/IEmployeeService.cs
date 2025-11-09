using ReferenceWebApi.Application.Dtos.Employee;

namespace ReferenceWebApi.Application.Interfaces
{
    public interface IEmployeeService : IServiceBase<EmployeeDto, CreateEmployeeDto, UpdateEmployeeDto>
    {
        Task<EmployeeDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);
    }
}
