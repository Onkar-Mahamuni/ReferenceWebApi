using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ReferenceWebApi.Infrastructure.Data
{
    /// Design-time factory for EF Core migrations
    /// Used by: dotnet ef migrations add/update commands
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Connection string for design-time only (migrations)
            // Update this to match your local SQL Server instance
            optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ReferenceWebApiDb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        //public ApplicationDbContext CreateDbContext(string[] args)
        //{
        //    // Get the Infrastructure project directory
        //    var infrastructureDirectory = Directory.GetCurrentDirectory();

        //    // Navigate to the API project (assumes standard solution structure)
        //    var apiProjectPath = Path.Combine(infrastructureDirectory, "..", "ReferenceWebApi.Api");

        //    // Verify the path exists
        //    if (!Directory.Exists(apiProjectPath))
        //    {
        //        throw new DirectoryNotFoundException(
        //            $"API project not found at: {Path.GetFullPath(apiProjectPath)}");
        //    }

        //    var appsettingsPath = Path.Combine(apiProjectPath, "appsettings.json");
        //    if (!File.Exists(appsettingsPath))
        //    {
        //        throw new FileNotFoundException(
        //            $"appsettings.json not found at: {Path.GetFullPath(appsettingsPath)}");
        //    }

        //    var configuration = new ConfigurationBuilder()
        //        .SetBasePath(apiProjectPath)
        //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        //        .Build();

        //    var connectionString = configuration.GetConnectionString("DefaultConnection");

        //    if (string.IsNullOrEmpty(connectionString))
        //    {
        //        throw new InvalidOperationException(
        //            "Connection string 'DefaultConnection' not found in appsettings.json");
        //    }

        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseSqlServer(connectionString);

        //    return new ApplicationDbContext(optionsBuilder.Options);
        //}
    }
}