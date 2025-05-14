using GameOfLife.Business.Domain.Entities;

namespace GameOfLife.Business.Domain.Interfaces;

public interface IBoardStateCacheRepository
{
    BoardState? Get(Guid boardId, int generation);
    void Set(Guid boardId, int generation, BoardState boardState);
}