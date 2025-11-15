using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReferenceWebApi.Domain.Entities;

namespace ReferenceWebApi.Infrastructure.Data.Configuration
{
    public class EmployeeConfiguration : EntityBaseConfiguration<Employee>
    {
        public override void Configure(EntityTypeBuilder<Employee> builder)
        {
            // Apply base entity configuration (Id, audit fields, indexes)
            base.Configure(builder);

            // Table name
            builder.ToTable("Employees");

            // Employee-specific properties
            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

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

            // Employee-specific indexes
            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_Employees_Email");

            builder.HasIndex(e => e.Department)
                .HasDatabaseName("IX_Employees_Department");

            builder.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Employees_IsActive");
        }
    }
}