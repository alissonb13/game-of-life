using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Business.UseCases.GetNextBoardState;

public class GetNextBoardStateUseCase(
    IBoardRepository repository,
    IBoardStateManagementService service,
    ILogger<GetNextBoardStateUseCase> logger) : IGetNextBoardState
{
    public async Task<GetNextBoardStateOutput> Execute(GetNextBoardStateInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var board = await repository.GetByIdAsync(input.Id) ?? throw new BoardNotFoundException(input.Id);

        logger.LogInformation("Getting next state for board {boardId}", board.Id);

        var nextState = service.GetNextState(board.CurrentState);
        logger.LogInformation("New state for board {boardId}: {newState}", board.Id, nextState);

        board.AddState(nextState);
        logger.LogInformation("New state added to board {boardId}", board.Id);

        await repository.UpdateAsync(board);
        logger.LogInformation("Board {boardId} updated", board.Id);

        return new GetNextBoardStateOutput(board.Id, nextState);
    }
}