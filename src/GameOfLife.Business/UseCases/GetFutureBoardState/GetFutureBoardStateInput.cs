using GameOfLife.Business.Domain.Exceptions;

namespace GameOfLife.Business.UseCases.GetFutureBoardState;

public record GetFutureBoardStateInput(Guid Id, int FutureStates)
{
    public void Validate()
    {
        if (FutureStates < 1)
        {
            throw new InvalidFutureStateException(FutureStates);
        }
    }
}