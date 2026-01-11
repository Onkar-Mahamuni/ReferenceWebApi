using Microsoft.Extensions.Logging;
using ReferenceWebApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceWebApi.Infrastructure.Data
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                if (!context.Employees.Any())
                {
                    await context.Employees.AddRangeAsync(
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "john.doe@company.com",
                            Department = "IT",
                            Position = "Software Engineer",
                            Salary = 75000,
                            HireDate = DateTime.UtcNow.AddYears(-2),
                            IsActive = true,
                            CreatedBy = "System"
                        },
                        new Employee
                        {
                            FirstName = "Jane",
                            LastName = "Smith",
                            Email = "jane.smith@company.com",
                            Department = "HR",
                            Position = "HR Manager",
                            Salary = 85000,
                            HireDate = DateTime.UtcNow.AddYears(-3),
                            IsActive = true,
                            CreatedBy = "System"
                        });

                    await context.SaveChangesAsync();
                    logger.LogInformation("Database seeded successfully");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
}
