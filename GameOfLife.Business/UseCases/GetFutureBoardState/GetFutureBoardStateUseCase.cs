using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Business.UseCases.GetFutureBoardState;

public class GetFutureBoardStateUseCase(IBoardRepository repository, ILogger<GetFutureBoardStateUseCase> logger) : IGetFutureBoardState
{
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
            var nextState = board.CurrentState.GetNextState();
            logger.LogInformation("New state for board {boardId}: {newState}", board.Id, nextState);
            
            board.AddState(nextState);
            logger.LogInformation("New state added to board {boardId}", board.Id);
        }

        await repository.UpdateAsync(board);
        logger.LogInformation("Board {boardId} updated", board.Id);

        return new GetFutureBoardStateOutput(board.Id, board.CurrentState);
    }
}