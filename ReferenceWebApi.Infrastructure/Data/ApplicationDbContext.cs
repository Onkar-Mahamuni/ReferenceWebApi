using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ReferenceWebApi.Application.Interfaces;
using ReferenceWebApi.Domain.Entities;

namespace ReferenceWebApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IUserContextService? _userContextService;

        // Constructor for runtime (with DI)
        public ApplicationDbContext(
            DbContextOptions options,
            IUserContextService userContextService)
            : base(options)
        {
            _userContextService = userContextService;
        }

        // Constructor for design-time (migrations)
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
            // _userContextService will be null during migrations
        }

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
            var username = _userContextService?.Username ?? "System";
            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = username;
                        Console.WriteLine($"Set CreatedBy: {username}");
                        break;

                    case EntityState.Modified:
                        // Only update these fields if it's NOT a soft delete
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = username;
                        Console.WriteLine($"Set UpdatedBy: {username}");
                        break;

                    case EntityState.Deleted:
                        // Convert hard delete to soft delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = now;
                        entry.Entity.DeletedBy = username;
                        Console.WriteLine($"Set DeletedBy: {username}");

                        //// Explicitly mark these properties as modified so that EF core will not override these values
                        //entry.Property(nameof(EntityBase.IsDeleted)).IsModified = true;
                        //entry.Property(nameof(EntityBase.DeletedAt)).IsModified = true;
                        //entry.Property(nameof(EntityBase.DeletedBy)).IsModified = true;
                        break;
                }
            }
        }
    }
}
