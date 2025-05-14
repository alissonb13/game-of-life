namespace GameOfLife.Business.UseCases.GetLastBoardState;

public interface IGetLastBoardState
{
    Task<GetLastBoardStateOutput> Execute(GetLastBoardStateInput input);
}