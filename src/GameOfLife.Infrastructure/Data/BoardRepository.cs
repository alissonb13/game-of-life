using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.Domain.Entities;

namespace GameOfLife.Infrastructure.Data;


/// <summary>
/// Repository implementation for managing Board entities using the GameOfLifeContext.
/// </summary>
public class BoardRepository(GameOfLifeContext context) : IBoardRepository
{
    /// <summary>
    /// Retrieves a board by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the board.</param>
    /// <returns>The board entity if found; otherwise, null.</returns>
    public async Task<Board?> GetByIdAsync(Guid id)
    {
        return await context.FindAsync<Board>(id);
    }

    /// <summary>
    /// Updates an existing board entity in the database.
    /// </summary>
    /// <param name="board">The board entity to update.</param>
    public async Task UpdateAsync(Board board)
    {
        context.Boards.Update(board);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Persists a new board entity to the database.
    /// </summary>
    /// <param name="board">The board entity to save.</param>
    public async Task SaveAsync(Board board)
    {
        await context.Boards.AddAsync(board);
        await context.SaveChangesAsync();
    }
}