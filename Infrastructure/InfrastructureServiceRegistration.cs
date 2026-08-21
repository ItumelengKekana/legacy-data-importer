using Application.Common.Interfaces;
using Domain.Customers;
using Domain.Imports;
using Domain.Orders;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IImportLogRepository, ImportLogRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddTransient<IFileParser, FileParser>();

        return services;
    }
}
