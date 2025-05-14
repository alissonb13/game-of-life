using GameOfLife.Business.Domain.Entities;

namespace GameOfLife.Business.Domain.Interfaces;

public interface IBoardService
{
    Task CreateAsync(Board board);
    Task UpdateAsync(Board board);
    Task<Board> GetByIdAsync(Guid boardId);
    Board? GetExistingStateFromBoardByGeneration(Board board, int generation);
}