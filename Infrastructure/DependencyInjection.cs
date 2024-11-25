using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Core.Request;
using FluentValidation;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Validation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

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
        services.AddAuth(configuration);
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISimulateService, SimulateService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISimulateRepository, SimulateRepository>();
        services.AddScoped<IBankRepository, BankRepository>();
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
        services.AddScoped<IValidator<LoanSimulateRequest>, SimulateLoanValidation>();
        services.AddScoped<IValidator<LoanApplicationRequest>, LoanApplicationValidation>();
        services.AddScoped<IValidator<LoanRejectRequest>, LoanRejectValidation>();
        services.AddScoped<IValidator<InstallmentPaymentRequest>, InstallmentPaymentValidation>();
        return services;
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        configuration.GetSection("JWT").Get<JwtConfig>();

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.Secret))
            };
        });

        services.AddTransient<AuthService>();

        return services;
    }
}
