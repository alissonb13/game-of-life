using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.UseCases.CreateBoard;
using Moq;

namespace GameOfLife.Tests.Business.UseCases;

public class CreateBoardUseCaseTest
{
    [Fact]
    public async Task Execute_WithValidInput_ShouldCreateBoardAndReturnOutput()
    {
        var repo = new Mock<IBoardRepository>();

        var input = CreateBoardInput.Create(new int[][]
        {
            [1, 0],
            [0, 1]
        });

        var usecase = new CreateBoardUseCase(repo.Object);

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

        repo.Verify(r => r.SaveAsync(It.IsAny<Board>()), Times.Once);
    }

    [Fact]
    public async Task Execute_WithNullInput_ShouldThrowArgumentNullException()
    {
        var repo = new Mock<IBoardRepository>();
        var usecase = new CreateBoardUseCase(repo.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => usecase.Execute(null!));
    }
}