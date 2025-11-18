using LonelyInterview.Domain.Entities;
using LonelyInterview.Domain.Repository;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace LonelyInterview.Infrastructure.Data;

public static class ConfigureDatabase
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration _configuration)
    {
        
        return services.AddDbContextPool<LonelyInterviewContext>(options =>
        {
            options.UseNpgsql(_configuration["ConnectionStrings:default"], opts =>
            {
                opts.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                opts.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "LonelyInterview");

            }).EnableSensitiveDataLogging();
        });

    }

    public static IServiceCollection AddDataSources(this IServiceCollection services) =>
        services
        .AddScoped<CandidateDataSource>()
        .AddScoped<HrManagerDataSource>()
        .AddScoped<LonelyInterviewUnitOfWork>()
        .AddScoped<VacancyDataSource>();



    public static IServiceCollection AddDatabaseUserContext(this IServiceCollection services, IConfiguration _configuration)
    {
        return services.AddDbContextPool<ApplicationUserContext>(options =>
        {
            options.UseNpgsql(_configuration["ConnectionStrings:default"], opts =>
            {
                opts.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                opts.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Auth");
            });

        });

    }

}