using Microsoft.Extensions.DependencyInjection;
using Safety_Tech.Services.UserServices;
using Safety_Tech.Services.RoleServices;
using Safety_Tech.Services.CSVFile;

namespace Safety_Tech.Services
{
    /// <summary>
    /// Extension methods for registering application services with the DI container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all application services for dependency injection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            // Add more services here as needed
            services.AddScoped<ICSVFile, Safety_Tech.Services.CSVFile.CSVFile>();
            return services;
        }
    }
} 