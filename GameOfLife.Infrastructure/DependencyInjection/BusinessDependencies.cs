using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.Domain.Services;
using GameOfLife.Business.UseCases.CreateBoard;
using GameOfLife.Business.UseCases.GetFutureBoardState;
using GameOfLife.Business.UseCases.GetLastBoardState;
using GameOfLife.Business.UseCases.GetNextBoardState;
using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife.Infrastructure.DependencyInjection;

public static class BusinessDependencies
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        return services
            .AddScoped<ICreateBoard, CreateBoardUseCase>()
            .AddScoped<IGetNextBoardState, GetNextBoardStateUseCase>()
            .AddScoped<IGetFutureBoardState, GetFutureBoardStateUseCase>()
            .AddScoped<IGetLatestBoardState, GetLatestBoardStateUseCase>();
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IBoardService, BoardService>()
            .AddScoped<IBoardStateManagementService, BoardStateManagementService>();
    }
}