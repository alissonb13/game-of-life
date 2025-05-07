using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;

namespace GameOfLife.Business.UseCases.GetFutureBoardState;

public class GetFutureBoardStateUseCase(IBoardRepository repository) : IGetFutureBoardState
{
    public async Task<GetFutureBoardStateOutput> Execute(GetFutureBoardStateInput input)
    {
        if (input.FutureStates < 1)
        {
            throw new InvalidFutureStateException(input.FutureStates);
        }

        var board = await repository.GetByIdAsync(input.Id) ?? throw new BoardNotFoundException(input.Id);

        for (var state = 0; state < input.FutureStates; state++)
        {
            var nextState = board.CurrentState.GetNextState();
            board.AddState(nextState);
        }

        await repository.UpdateAsync(board);

        return new GetFutureBoardStateOutput(board.Id, board.CurrentState);
    }
}