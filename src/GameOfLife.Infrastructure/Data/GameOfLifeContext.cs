using System.Reflection;
using GameOfLife.Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infrastructure.Data;

public class GameOfLifeContext(DbContextOptions<GameOfLifeContext> options) : DbContext(options)
{
    public DbSet<Board> Boards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*
         * Using the modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()) method is essential for automatically
         * discovering all classes within the assembly that implement the IEntityTypeConfiguration<T> interface, ensuring that Entity
         * Framework Core applies the appropriate configurations when generating the data model.
         *
         * Although it is possible to manually register each configuration using modelBuilder.ApplyConfiguration(new BoardEntityMapper()),
         * this approach is not recommended for production environments. This method requires explicitly adding each mapping, which can
         * lead to omissions and, consequently, errors during migrations or runtime behavior.
         *
         * Therefore, the best practice is to automatically apply all configurations found within the executing assembly. This ensures
         * that all configuration classes are included, even if they are not explicitly registered in the OnModelCreating method. This
         * approach results in cleaner, more consistent code, and aligns better with the Single Responsibility Principle and scalability.
         *
         * For further reading on this topic, check out the article:
         * https://medium.com/@josiahmahachi/single-responsibility-principle-in-entity-framework-configurations-d86012eeab44
         */
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}