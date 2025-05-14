using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Business.UseCases.GetFutureBoardState;

/// <summary>
/// Handles the use case for retrieving future states of a board in the Game of Life.
/// </summary>
/// <param name="repository">Repository for persisting board data.</param>
/// <param name="service">Service to manage the board state.</param>
/// <param name="logger">Logger instance for logging operations.</param>
public class GetFutureBoardStateUseCase(
    IBoardRepository repository,
    IBoardStateManagementService service,
    ILogger<GetFutureBoardStateUseCase> logger) : IGetFutureBoardState
{
    
    /// <summary>
    /// Calculates and retrieves the future state(s) of a board.
    /// </summary>
    /// <param name="input">The input containing the board ID and number of future states to generate.</param>
    /// <returns>The board's updated current state after applying the requested number of future generations.</returns>
    /// <exception cref="InvalidFutureStateException">Thrown when the requested number of future states is less than 1.</exception>
    /// <exception cref="BoardNotFoundException">Thrown when the board with the given ID is not found.</exception>
    public async Task<GetFutureBoardStateOutput> Execute(GetFutureBoardStateInput input)
    {
        if (input.FutureStates < 1)
        {
            logger.LogError("FutureStates must be greater than 0");
            throw new InvalidFutureStateException(input.FutureStates);
        }

        var board = await repository.GetByIdAsync(input.Id) ?? throw new BoardNotFoundException(input.Id);

        logger.LogInformation("Getting future state for board {boardId}", board.Id);

        for (var state = 0; state < input.FutureStates; state++)
        {
            var nextState = service.GetNextState(board.CurrentState);
            logger.LogInformation("New state for board {boardId}: {newState}", board.Id, nextState);

            board.AddState(nextState);
            logger.LogInformation("New state added to board {boardId}", board.Id);
        }

        await repository.UpdateAsync(board);
        logger.LogInformation("Board {boardId} updated", board.Id);

        return new GetFutureBoardStateOutput(board.Id, board.CurrentState);
    }
}