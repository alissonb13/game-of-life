using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Business.UseCases.GetLastBoardState;

public class GetLatestBoardStateUseCase(
    IBoardRepository repository,
    IBoardStateManagementService service,
    ILogger<GetLatestBoardStateUseCase> logger) : IGetLatestBoardState
{
    public async Task<GetLatestBoardStateOutput> Execute(GetLatestBoardStateInput input)
    {
        var board = await repository.GetByIdAsync(input.BoardId)
                    ?? throw new BoardNotFoundException(input.BoardId);

        logger.LogInformation("Getting latest state for board {boardId}", board.Id);

        for (var state = 0; state < input.GenerationMaxValue; state++)
        {
            var nextState = service.GetNextState(board.CurrentState);
            logger.LogInformation("New state for board {boardId}: {newState}", board.Id, nextState);

            board.AddState(nextState);
            logger.LogInformation("New state added to board {boardId}", board.Id);

            if (!board.IsConcluded()) continue;

            logger.LogInformation("Board {boardId} is concluded", board.Id);
            break;
        }

        await repository.UpdateAsync(board);
        logger.LogInformation("Board {boardId} updated", board.Id);

        return new GetLatestBoardStateOutput(board);
    }
}