using GameOfLife.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife.Tests.Helpers;


public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GameOfLifeContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<GameOfLifeContext>(options =>
            {
                options.UseInMemoryDatabase("GameOfLifeTestDb");
            });

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GameOfLifeContext>();
            db.Database.EnsureCreated();
        });
    }
}






