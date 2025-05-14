using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Business.UseCases.GetNextBoardState;

/// <summary>
/// Handles the use case for retrieving the next state of a specific board in the Game of Life.
/// </summary>
/// <param name="boardService">Service to handle board data.</param>
/// <param name="boardStateManagementService">Service to manage the board state.</param>
/// <param name="logger">Logger instance for logging operations.</param>
public class GetNextBoardStateUseCase(
    IBoardService boardService,
    IBoardStateManagementService boardStateManagementService,
    ILogger<GetNextBoardStateUseCase> logger) : IGetNextBoardState
{
    /// <summary>
    /// Computes and returns the next state of a board based on its current configuration.
    /// </summary>
    /// <param name="input">The input containing the board ID.</param>
    /// <returns> A GetNextBoardStateOutput containing the board ID and the newly computed state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input is null.</exception>
    /// <exception cref="BoardNotFoundException">Thrown when the board with the specified ID does not exist.</exception>
    public async Task<GetNextBoardStateOutput> Execute(GetNextBoardStateInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var board = await boardService.GetByIdAsync(input.Id);

        logger.LogInformation("Getting next state for board {boardId}", board.Id);

        var existingState = boardService
            .GetExistingStateFromBoardByGeneration(board, board.CurrentState.Generation + 1);

        if (existingState != null) return new GetNextBoardStateOutput(board.Id, existingState.CurrentState);

        var nextState = boardStateManagementService.GetNextState(board.CurrentState);
        logger.LogInformation("New state for board {boardId}: {newState}", board.Id, nextState);

        board.AddState(nextState);
        logger.LogInformation("New state added to board {boardId}", board.Id);

        await boardService.UpdateAsync(board);
        logger.LogInformation("Board {boardId} updated", board.Id);

        return new GetNextBoardStateOutput(board.Id, nextState);
    }
}