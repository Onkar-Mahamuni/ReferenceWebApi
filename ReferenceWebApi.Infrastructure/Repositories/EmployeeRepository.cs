using Microsoft.EntityFrameworkCore;
using ReferenceWebApi.Domain.Entities;
using ReferenceWebApi.Domain.Interfaces;
using ReferenceWebApi.Infrastructure.Data;

namespace ReferenceWebApi.Infrastructure.Repositories
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(
            string department,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => e.Department == department)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(
            string email,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(e => e.Email == email);

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
}
