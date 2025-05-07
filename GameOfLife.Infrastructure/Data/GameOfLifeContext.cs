using System.Reflection;
using GameOfLife.Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infrastructure.Data;

public class GameOfLifeContext(DbContextOptions<GameOfLifeContext> options) : DbContext(options)
{
    public DbSet<Board> Boards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}