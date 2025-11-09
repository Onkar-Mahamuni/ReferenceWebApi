using ReferenceWebApi.Domain.Entities;

namespace ReferenceWebApi.Domain.Interfaces
{
    public interface IEmployeeRepository : IRepositoryBase<Employee>
    {
        Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}
