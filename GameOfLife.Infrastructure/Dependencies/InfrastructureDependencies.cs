using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace GameOfLife.Infrastructure.Dependencies;

public static class InfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(
        this IServiceCollection services,
        IConfigurationManager configuration)
    {
        return services.AddDbContext<GameOfLifeContext>(options =>
            options.UseNpgsql(new NpgsqlDataSourceBuilder(
                configuration.GetConnectionString("DefaultConnection")
            ).EnableDynamicJson().Build())
        ).AddScoped<IBoardRepository, BoardRepository>();
    }
}