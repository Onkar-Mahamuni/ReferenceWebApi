using System.Reflection;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using ReferenceWebApi.Api.Filters;
using ReferenceWebApi.Application.Interfaces;
using ReferenceWebApi.Application.Services;
using ReferenceWebApi.Domain.Interfaces;
using ReferenceWebApi.Infrastructure.Data;
using ReferenceWebApi.Infrastructure.Repositories;
using ReferenceWebApi.Api.HealthChecks;

namespace ReferenceWebApi.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(config => 
            { 
                config.AddMaps(typeof(Application.Mappings.MappingProfile).Assembly); 
            });

            // FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Program>();

            // Services
            services.AddScoped<IEmployeeService, EmployeeService>();

            // Filters
            services.AddScoped<ValidationFilter>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // HttpContextAccessor
            services.AddHttpContextAccessor();

            // User Context Service
            services.AddScoped<IUserContextService, UserContextService>();

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            //services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

            return services;
        }

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            // API Versioning
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CompanyName.ProjectName API",
                    Version = "v1",
                    Description = "Enterprise CRUD API",
                    Contact = new OpenApiContact
                    {
                        Name = "Development Team",
                        Email = "dev@company.com"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Health Checks
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>()
                .AddCheck<DatabaseMigrationHealthCheck>("database_migrations");

            return services;
        }
    }
}
