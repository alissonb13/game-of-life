using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Infrastructure.Cache;
using GameOfLife.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace GameOfLife.Infrastructure.DependencyInjection;

public static class InfrastructureDependencies
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IBoardRepository, BoardRepository>();
    }

    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        return services
            .AddMemoryCache()
            .AddScoped<ICacheProvider, MemoryCacheProvider>();
    }

    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddDbContext<GameOfLifeContext>(options =>
                options.UseNpgsql(new NpgsqlDataSourceBuilder(
                    configuration.GetConnectionString("DefaultConnection")
                ).EnableDynamicJson().Build())
            );
    }
}