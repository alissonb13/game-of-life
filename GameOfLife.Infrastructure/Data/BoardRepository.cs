using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.Domain.Entities;

namespace GameOfLife.Infrastructure.Data;

public class BoardRepository(GameOfLifeContext context) : IBoardRepository
{
    public async Task<Board?> GetByIdAsync(Guid id)
    {
        return await context.FindAsync<Board>(id);
    }

    public async Task UpdateAsync(Board board)
    {
        context.Boards.Update(board);
        await context.SaveChangesAsync();
    }

    public async Task SaveAsync(Board board)
    {
        await context.Boards.AddAsync(board);
        await context.SaveChangesAsync();
    }
}