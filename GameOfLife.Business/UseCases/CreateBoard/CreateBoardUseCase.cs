using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Extensions;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Business.UseCases.CreateBoard;

/// <summary>
/// Use case for creating a new game board.
/// </summary>
/// <param name="boardService">Service to handle board data.</param>
/// <param name="logger">Logger instance for logging operations.</param>
public class CreateBoardUseCase(
    IBoardService boardService,
    ILogger<CreateBoardUseCase> logger) : ICreateBoard
{
    /// <summary>
    /// Executes the creation of a new board from the provided input grid.
    /// </summary>
    /// <param name="input">Input data containing the initial grid.</param>
    /// <returns>The output containing the created board.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input is null.</exception>
    public async Task<CreateBoardOutput> Execute(CreateBoardInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        logger.LogInformation("Starting create board");

        var initialState = BoardState.Create(input.Grid.ToCellState());
        var board = Board.Create(initialState);

        await boardService.CreateAsync(board);

        logger.LogInformation("New board created");

        return new CreateBoardOutput(board);
    }
}