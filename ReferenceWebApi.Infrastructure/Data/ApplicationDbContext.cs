using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ReferenceWebApi.Domain.Entities;

namespace ReferenceWebApi.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees => Set<Employee>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Global query filter for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(GenerateSoftDeleteFilter(entityType.ClrType));
                }
            }
        }

        private static LambdaExpression GenerateSoftDeleteFilter(Type type)
        {
            var parameter = Expression.Parameter(type, "e");
            var property = Expression.Property(parameter, nameof(EntityBase.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            return Expression.Lambda(condition, parameter);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<EntityBase>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}
