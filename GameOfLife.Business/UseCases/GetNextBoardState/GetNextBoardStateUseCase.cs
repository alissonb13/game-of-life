using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;

namespace GameOfLife.Business.UseCases.GetNextBoardState;

public class GetNextBoardStateUseCase(IBoardRepository boardRepository) : IGetNextBoardState
{
    public async Task<GetNextBoardStateOutput> Execute(GetNextBoardStateInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        
        var board = await boardRepository.GetByIdAsync(input.Id) ?? throw new BoardNotFoundException(input.Id);

        var nextState = board.CurrentState.GetNextState();

        board.AddState(nextState);

        await boardRepository.UpdateAsync(board);

        return new GetNextBoardStateOutput(board.Id, nextState);
    }
}