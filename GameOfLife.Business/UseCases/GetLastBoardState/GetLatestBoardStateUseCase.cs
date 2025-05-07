using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;

namespace GameOfLife.Business.UseCases.GetLastBoardState;

public class GetLatestBoardStateUseCase(IBoardRepository repository) : IGetLatestBoardState
{
    public async Task<GetLatestBoardStateOutput> Execute(GetLatestBoardStateInput input)
    {
        var board = await repository.GetByIdAsync(input.BoardId)
                    ?? throw new BoardNotFoundException(input.BoardId);

        for (var state = 0; state < input.GenerationMaxValue; state++)
        {
            var nextState = board.CurrentState.GetNextState();
            board.AddState(nextState);

            if (board.IsConcluded()) break;
        }

        await repository.UpdateAsync(board);

        return new GetLatestBoardStateOutput(board);
    }
}