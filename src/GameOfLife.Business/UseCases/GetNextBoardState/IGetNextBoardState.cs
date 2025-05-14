namespace GameOfLife.Business.UseCases.GetNextBoardState;

public interface IGetNextBoardState
{
    Task<GetNextBoardStateOutput> Execute(GetNextBoardStateInput input);
}