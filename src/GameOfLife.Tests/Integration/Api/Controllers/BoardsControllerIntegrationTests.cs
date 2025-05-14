using GameOfLife.Api.Controllers;
using GameOfLife.Api.DTOs.Requests;
using GameOfLife.Api.DTOs.Responses;
using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.Domain.Services;
using GameOfLife.Business.UseCases.CreateBoard;
using GameOfLife.Business.UseCases.GetFutureBoardState;
using GameOfLife.Business.UseCases.GetLastBoardState;
using GameOfLife.Business.UseCases.GetNextBoardState;
using GameOfLife.Infrastructure.Cache;
using GameOfLife.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Tests.Integration.Api.Controllers;

public class BoardsControllerIntegrationTests : IAsyncLifetime
{
    private readonly ServiceProvider _serviceProvider;
    private readonly BoardsController _controller;
    private readonly GameOfLifeContext _dbContext;

    public BoardsControllerIntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddDbContext<GameOfLifeContext>(options => options.UseInMemoryDatabase("GameOfLifeContext"));
        services.AddMemoryCache();

        services.AddScoped<ICreateBoard, CreateBoardUseCase>();
        services.AddScoped<IGetNextBoardState, GetNextBoardStateUseCase>();
        services.AddScoped<IGetFutureBoardState, GetFutureBoardStateUseCase>();
        services.AddScoped<IGetLastBoardState, GetLastBoardStateUseCase>();

        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IBoardStateManagementService, BoardStateManagementService>();

        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<ICacheProvider, MemoryCacheProvider>();

        services.AddLogging();

        _serviceProvider = services.BuildServiceProvider();

        _dbContext = _serviceProvider.GetRequiredService<GameOfLifeContext>();
        var createBoard = _serviceProvider.GetRequiredService<ICreateBoard>();
        var getNextBoardState = _serviceProvider.GetRequiredService<IGetNextBoardState>();
        var getFutureBoardState = _serviceProvider.GetRequiredService<IGetFutureBoardState>();
        var getLatestBoardState = _serviceProvider.GetRequiredService<IGetLastBoardState>();
        var logger = _serviceProvider.GetRequiredService<ILogger<BoardsController>>();

        _controller = new BoardsController(
            createBoard,
            getNextBoardState,
            getFutureBoardState,
            getLatestBoardState,
            logger
        );
    }

    public async Task InitializeAsync()
    {
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task UploadBoard_WithValidRequest_ShouldCreateBoardAndReturnId()
    {
        var request = new CreateBoardRequest
        {
            Grid = new int[][]
            {
                [0, 1, 0],
                [0, 1, 0],
                [0, 1, 0]
            }
        };

        var result = await _controller.UploadBoard(request);

        var createdResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<CreateBoardResponse>(createdResult.Value);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);

        var boardInDb = await _dbContext.Boards
            .FirstOrDefaultAsync(b => b.Id == response.Id);
            
        Assert.NotNull(boardInDb);
        Assert.Single(boardInDb.History); 
        Assert.Equal(3, boardInDb.Columns);
    }

    [Fact]
    public async Task GetNextBoardState_WithValidBoardId_ShouldReturnNextState()
    {
        var grid = new CellState[][]
        {
            [CellState.Dead, CellState.Alive, CellState.Dead],
            [CellState.Dead, CellState.Alive, CellState.Dead],
            [CellState.Dead, CellState.Alive, CellState.Dead]
        };
            
        var initialState = BoardState.Create(grid);
        var board = Board.Create(initialState);
        await _dbContext.Boards.AddAsync(board);
        await _dbContext.SaveChangesAsync();

        var result = await _controller.GetNextBoardState(board.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var output = Assert.IsType<GetNextBoardStateOutput>(okResult.Value);

        Assert.Equal(3, output.State.Grid.Length);
        Assert.Equal(3, output.State.Grid[1].Length);
    }

    [Fact]
    public async Task GetFutureBoardState_WithValidBoardId_ShouldReturnFutureState()
    {
        var grid = new CellState[][]
        {
            [CellState.Dead, CellState.Alive, CellState.Dead],
            [CellState.Dead, CellState.Alive, CellState.Dead],
            [CellState.Dead, CellState.Alive, CellState.Dead]
        };
            
        var initialState = BoardState.Create(grid);
        var board = Board.Create(initialState);
        await _dbContext.Boards.AddAsync(board);
        await _dbContext.SaveChangesAsync();

        var result = await _controller.GetFutureBoardState(board.Id, 2);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var output = Assert.IsType<GetFutureBoardStateOutput>(okResult.Value);

        Assert.Equal(CellState.Dead, output.State.Grid[0][0]);
        Assert.Equal(CellState.Alive, output.State.Grid[0][1]);
        Assert.Equal(CellState.Dead, output.State.Grid[0][2]);
    }

    [Fact]
    public async Task GetLatestBoardState_WithValidBoardId_ShouldReturnStableState()
    {
        var grid = new CellState[][]
        {
            [CellState.Alive, CellState.Alive],
            [CellState.Alive, CellState.Alive]
        };
            
        var initialState = BoardState.Create(grid);
        var board = Board.Create(initialState);
        await _dbContext.Boards.AddAsync(board);
        await _dbContext.SaveChangesAsync();

        var result = await _controller.GetLatestBoardState(board.Id, 10);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<GetLastBoardStateOutput>(okResult.Value);
        Assert.True(board.IsConcluded());
    }

    [Fact]
    public async Task GetNextBoardState_WithInvalidBoardId_ShouldReturnBadRequest()
    {
        var result = await _controller.GetNextBoardState(Guid.Empty);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }
}
