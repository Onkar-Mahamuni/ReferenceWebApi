using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using ReferenceWebApi.Infrastructure.Data;

namespace ReferenceWebApi.Api.HealthChecks
{
    public class DatabaseMigrationHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _context;

        public DatabaseMigrationHealthCheck(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
                var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync(cancellationToken);

                if (pendingMigrations.Any())
                {
                    return HealthCheckResult.Degraded(
                        $"Database has {pendingMigrations.Count()} pending migration(s)",
                        data: new Dictionary<string, object>
                        {
                            { "pendingMigrations", pendingMigrations.ToList() },
                            { "appliedMigrations", appliedMigrations.Count() }
                        });
                }

                return HealthCheckResult.Healthy(
                    "Database is up to date",
                    data: new Dictionary<string, object>
                    {
                        { "appliedMigrations", appliedMigrations.Count() }
                    });
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Failed to check database migrations",
                    exception: ex);
            }
        }
    }
}