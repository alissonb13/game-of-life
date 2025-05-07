namespace GameOfLife.Business.UseCases.GetFutureBoardState;

public interface IGetFutureBoardState
{
    Task<GetFutureBoardStateOutput> Execute(GetFutureBoardStateInput input);
}