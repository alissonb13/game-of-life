using AutoFixture;
using GameOfLife.Business.Domain.Entities;
using GameOfLife.Tests.Helpers;

namespace GameOfLife.Tests.Business.Domain.Entities;

public class BoardTest
{
    private readonly IFixture _fixture = new Fixture();

    [Fact]
    public void Create_ShouldInitializeWithCorrectProperties()
    {
        var rows = _fixture.Create<int>() % 10 + 1;
        var cols = _fixture.Create<int>() % 10 + 1;
        var grid = GridHelper.CreateGrid(rows, cols);
        var initialState = BoardState.Create(grid);

        var board = Board.Create(initialState);

        Assert.Equal(cols, board.Columns);
        Assert.Equal(rows, board.Rows);
        Assert.Single(board.History);
        Assert.Equal(initialState, board.CurrentState);
    }

    [Fact]
    public void Board_ShouldThrowsException_WhenTryToCreateWithNullBoardStateValue()
    {
        Assert.Throws<ArgumentNullException>(() => Board.Create(null!));
    }
}