using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Interfaces;

namespace GameOfLife.Infrastructure.Data;

public class BoardStateCacheRepository(ICacheProvider cacheProvider) : IBoardStateCacheRepository
{
    public BoardState? Get(Guid boardId, int generation)
    {
        return cacheProvider.Get<BoardState?>(GenerateKey(boardId, generation));
    }

    public void Set(Guid boardId, int generation, BoardState boardState)
    {
        cacheProvider.Set(GenerateKey(boardId, generation), boardState);
    }

    private static string GenerateKey(Guid boardId, int generation)
    {
        return $"board_state:{boardId}:{generation}";
    }
}