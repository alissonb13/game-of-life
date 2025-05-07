namespace GameOfLife.Business.UseCases.GetLastBoardState;

public interface IGetLatestBoardState
{
    Task<GetLatestBoardStateOutput> Execute(GetLatestBoardStateInput input);
}