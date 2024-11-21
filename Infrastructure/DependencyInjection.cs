using Core.Interfaces.Services;
using Core.Request;
using FluentValidation;
using Infrastructure.Context;
using Infrastructure.Services;
using Infrastructure.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddRepositories();
        services.AddDatabase(configuration);
        services.AddValidation();
        services.AddMapping();
        services.AddAuth();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISimulateService, SimulateService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var ConnectionStrings = configuration.GetConnectionString("conncetion");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(ConnectionStrings));

        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        //services.AddScoped<IValidator<LoanRequest>, SimulateLoanValidation>();
        services.AddValidatorsFromAssemblyContaining<SimulateLoanValidation>();
        return services;
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        return services;
    }



}
