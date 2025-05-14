using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Business.UseCases.GetLastBoardState;

/// <summary>
/// Handles the use case for computing and retrieving the latest state of a board
/// until it reaches a concluded state or the maximum number of generations.
/// </summary>
/// <param name="boardService">Service for handle board data.</param>
/// <param name="boardStateManagementService">Service to manage the board state.</param>
/// <param name="logger">Logger instance for logging operations.</param>
public class GetLastBoardStateUseCase(
    IBoardService boardService,
    IBoardStateManagementService boardStateManagementService,
    ILogger<GetLastBoardStateUseCase> logger) : IGetLastBoardState
{
    /// <summary>
    /// Computes the next states of the board until it reaches the maximum allowed generations
    /// or the simulation reaches a concluded (stable or oscillating) state.
    /// </summary>
    /// <param name="input">The input containing the board ID and generation max value.</param>
    /// <returns>The updated board wrapped in a GetLatestBoardStateOutput object.</returns>
    /// <exception cref="BoardNotFoundException">Thrown if the board with the given ID is not found.</exception>
    public async Task<GetLastBoardStateOutput> Execute(GetLastBoardStateInput input)
    {
        var board = await boardService.GetByIdAsync(input.BoardId);

        logger.LogInformation("Getting latest state for board {boardId}", board.Id);
        
        for (var state = 0; state < input.GenerationMaxValue; state++)
        {
            var nextState = boardStateManagementService.GetNextState(board.CurrentState);
            logger.LogInformation("New state for board {boardId}: {newState}", board.Id, nextState);

            board.AddState(nextState);
            logger.LogInformation("New state added to board {boardId}", board.Id);

            if (!board.IsConcluded()) continue;

            logger.LogInformation("Board {boardId} is concluded", board.Id);
            break;
        }

        await boardService.UpdateAsync(board);
        logger.LogInformation("Board {boardId} updated", board.Id);

        return new GetLastBoardStateOutput(board);
    }
}