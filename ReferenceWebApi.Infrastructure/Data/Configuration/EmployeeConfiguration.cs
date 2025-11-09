using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ReferenceWebApi.Domain.Entities;

namespace ReferenceWebApi.Infrastructure.Data.Configuration
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(e => e.Department)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Salary)
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(100);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            builder.HasIndex(e => e.Department);
            builder.HasIndex(e => e.IsActive);
            builder.HasIndex(e => e.CreatedAt);
        }
    }
}
