using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Extensions;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.UseCases.GetLastBoardState;
using GameOfLife.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameOfLife.Tests.Unit.Business.UseCases;

public class GetLatestBoardStateUseCaseUnitTests
{
    private readonly Mock<IBoardService> _boardServiceMock = new();
    private readonly Mock<IBoardStateManagementService> _boardStateManagementServiceMock = new();
    private readonly Mock<ILogger<GetLastBoardStateUseCase>> _loggerMock = new();

    private readonly GetLastBoardStateUseCase _useCase;

    public GetLatestBoardStateUseCaseUnitTests()
    {
        _useCase = new GetLastBoardStateUseCase(
            _boardServiceMock.Object,
            _boardStateManagementServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Execute_BoardNotFound_ThrowsBoardNotFoundException()
    {
        var boardId = Guid.NewGuid();
        var input = new GetLastBoardStateInput(boardId, 10);

        _boardServiceMock
            .Setup(r => r.GetByIdAsync(boardId))
            .ThrowsAsync(new BoardNotFoundException(boardId));

        await Assert.ThrowsAsync<BoardNotFoundException>(() => _useCase.Execute(input));
    }

    [Fact]
    public async Task Execute_StopsWhenIsConcluded_ReturnsEarly()
    {
        const int generationMaxValue = 5;
        var grids = Enumerable.Range(0, 5)
            .Select(_ => GridHelper.CreateGrid(3, 3))
            .ToList();

        var board = Board.Create(BoardState.Create(grids[0]));

        _boardServiceMock
            .Setup(r => r.GetByIdAsync(board.Id))
            .ReturnsAsync(board);
        for (var grid = 1; grid < grids.Count; grid++)
        {
            var nextState = BoardState.Create(grids[grid], grid);

            _boardStateManagementServiceMock
                .Setup(s => s.GetNextState(It.IsAny<BoardState>()))
                .Returns(nextState);
        }

        var input = new GetLastBoardStateInput(board.Id, generationMaxValue);

        var output = await _useCase.Execute(input);

        _boardServiceMock.Verify(r => r.UpdateAsync(board), Times.Once);

        Assert.NotNull(output);
        Assert.Equal(board, output.Board);
    }

    [Fact]
    public async Task Execute_ReachesGenerationMaxValue_WhenNotConcluded()
    {
        const int generationMaxValue = 5;
        var grids = Enumerable.Range(0, 5)
            .Select(_ => GridHelper.CreateGrid(3, 3))
            .ToList();

        var board = Board.Create(BoardState.Create(grids[0]));

        _boardServiceMock
            .Setup(r => r.GetByIdAsync(board.Id))
            .ReturnsAsync(board);

        for (var grid = 1; grid < grids.Count; grid++)
        {
            var nextState = BoardState.Create(grids[grid], grid);

            _boardStateManagementServiceMock
                .Setup(s => s.GetNextState(It.IsAny<BoardState>()))
                .Returns(nextState);
        }

        var input = new GetLastBoardStateInput(board.Id, generationMaxValue);

        var output = await _useCase.Execute(input);

        _boardServiceMock.Verify(r => r.UpdateAsync(board), Times.Once);

        Assert.NotNull(output);
    }
}