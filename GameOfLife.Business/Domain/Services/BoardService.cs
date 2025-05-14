using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;

namespace GameOfLife.Business.Domain.Services;

public class BoardService(
    ICacheProvider cacheProvider,
    IBoardRepository boardRepository) : IBoardService
{
    public async Task<Board> GetByIdAsync(Guid boardId)
    {
        return cacheProvider.Get<Board>(boardId.ToString())
               ?? await boardRepository.GetByIdAsync(boardId)
               ?? throw new BoardNotFoundException(boardId);
    }

    public async Task CreateAsync(Board board)
    {
        await boardRepository.SaveAsync(board);
        cacheProvider.Set(board.Id.ToString(), board);
    }

    public async Task UpdateAsync(Board board)
    {
        await boardRepository.UpdateAsync(board);
        cacheProvider.Set(board.Id.ToString(), board);
    }

    public Board? GetExistingStateFromBoardByGeneration(Board board, int generation)
    {
        var existingState = board.History.FirstOrDefault(state => state.Generation == generation);

        if (existingState is null) return null;

        return new Board(
            board.Id,
            existingState.GetGridRows(),
            existingState.GetGridColumns(),
            [existingState]
        );
    }
}