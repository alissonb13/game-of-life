using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Extensions;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Business.UseCases.CreateBoard;

public class CreateBoardUseCase(IBoardRepository repository, ILogger<CreateBoardUseCase> logger) : ICreateBoard
{
    public async Task<CreateBoardOutput> Execute(CreateBoardInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        
        logger.LogInformation("Starting create board");

        var initialState = BoardState.Create(input.Grid.ToCellState());
        var board = Board.Create(initialState);
        
        await repository.SaveAsync(board);

        logger.LogInformation("New board created");
        
        return new CreateBoardOutput(board);
    }
}