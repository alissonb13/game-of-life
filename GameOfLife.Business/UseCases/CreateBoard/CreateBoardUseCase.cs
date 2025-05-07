using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Extensions;

namespace GameOfLife.Business.UseCases.CreateBoard;

public class CreateBoardUseCase(IBoardRepository boardRepository) : ICreateBoard
{
    public async Task<CreateBoardOutput> Execute(CreateBoardInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var initialState = BoardState.Create(input.Grid.ToCellState());
        var board = Board.Create(initialState);

        await boardRepository.SaveAsync(board);

        return new CreateBoardOutput(board);
    }
}