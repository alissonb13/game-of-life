using GameOfLife.Business.UseCases.CreateBoard;
using GameOfLife.Business.UseCases.GetFutureBoardState;
using GameOfLife.Business.UseCases.GetLastBoardState;
using GameOfLife.Business.UseCases.GetNextBoardState;
using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife.Infrastructure.Dependencies;

public static class BusinessDependencies
{
    public static IServiceCollection AddBusinessDependencies(this IServiceCollection services)
    {
        return services
            .AddScoped<ICreateBoard, CreateBoardUseCase>()
            .AddScoped<IGetNextBoardState, GetNextBoardStateUseCase>()
            .AddScoped<IGetFutureBoardState, GetFutureBoardStateUseCase>()
            .AddScoped<IGetLatestBoardState, GetLatestBoardStateUseCase>();
    }
}