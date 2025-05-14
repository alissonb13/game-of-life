using GameOfLife.Business.Domain.Entities;

namespace GameOfLife.Business.Domain.Interfaces;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid guid);
    Task UpdateAsync(Board board);
    Task SaveAsync(Board board);    
}