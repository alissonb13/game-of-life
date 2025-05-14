using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.UseCases.CreateBoard;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameOfLife.Tests.Unit.Business.UseCases;

public class CreateBoardUseCaseUnitTests
{
    private readonly Mock<IBoardService> _boardServiceMock = new();
    private readonly Mock<ILogger<CreateBoardUseCase>> _loggerMock = new();

    [Fact]
    public async Task Execute_WithValidInput_ShouldCreateBoardAndReturnOutput()
    {
        var input = CreateBoardInput.Create(new int[][]
        {
            [1, 0],
            [0, 1]
        });

        var usecase = new CreateBoardUseCase(_boardServiceMock.Object, _loggerMock.Object);

        var result = await usecase.Execute(input);

        Assert.NotNull(result);
        Assert.NotNull(result.Board);
        Assert.Equal(2, result.Board.Rows);
        Assert.Equal(2, result.Board.Columns);
        Assert.Single(result.Board.History);

        var initialState = result.Board.CurrentState;
        Assert.Equal(CellState.Alive, initialState.Grid[0][0]);
        Assert.Equal(CellState.Dead, initialState.Grid[0][1]);
        Assert.Equal(CellState.Dead, initialState.Grid[1][0]);
        Assert.Equal(CellState.Alive, initialState.Grid[1][1]);

        _boardServiceMock.Verify(r => r.CreateAsync(It.IsAny<Board>()), Times.Once);
    }

    [Fact]
    public async Task Execute_WithNullInput_ShouldThrowArgumentNullException()
    {
        var usecase = new CreateBoardUseCase(_boardServiceMock.Object, _loggerMock.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(() => usecase.Execute(null!));
    }
}