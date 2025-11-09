using Microsoft.EntityFrameworkCore;
using ReferenceWebApi.Infrastructure.Data;
using Serilog;

namespace ReferenceWebApi.Api.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            var applyMigrations = app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

            if (!applyMigrations)
            {
                Log.Information("Auto-migration is disabled");
                return;
            }

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Check if there are pending migrations
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    Log.Information("Applying {Count} pending migration(s)...", pendingMigrations.Count());

                    foreach (var migration in pendingMigrations)
                    {
                        Log.Information("  - {Migration}", migration);
                    }

                    await context.Database.MigrateAsync();
                    Log.Information("Database migrations applied successfully");
                }
                else
                {
                    Log.Information("Database is up to date. No migrations to apply");
                }

                // Optional: Seed data
                await SeedDataAsync(context);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        private static async Task SeedDataAsync(ApplicationDbContext context)
        {
            // Add your seed data logic here
            // Example:
            // if (!await context.Employees.AnyAsync())
            // {
            //     await context.Employees.AddRangeAsync(/* seed data */);
            //     await context.SaveChangesAsync();
            //     Log.Information("Seed data added successfully");
            // }

            await Task.CompletedTask;
        }
    }
}