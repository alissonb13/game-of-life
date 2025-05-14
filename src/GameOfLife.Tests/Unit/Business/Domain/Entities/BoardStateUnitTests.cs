using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;

namespace GameOfLife.Tests.Unit.Business.Domain.Entities;

public class BoardStateUnitTests
{
    [Fact]
    public void Constructor_WithNullGrid_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BoardState(null!, 1));
    }

    [Fact]
    public void Create_ShouldSetGridAndGenerationCorrectly()
    {
        var grid = new[]
        {
            new[] { CellState.Dead, CellState.Alive },
            new[] { CellState.Alive, CellState.Dead }
        };

        var state = BoardState.Create(grid, 5);

        Assert.Equal(5, state.Generation);
        Assert.Equal(grid, state.Grid);
    }

    [Fact]
    public void GetGridRows_ShouldReturnCorrectNumberOfRows()
    {
        var grid = new[]
        {
            new[] { CellState.Dead, CellState.Dead },
            new[] { CellState.Dead, CellState.Dead },
        };
        var state = BoardState.Create(grid);

        Assert.Equal(2, state.GetGridRows());
    }

    [Fact]
    public void GetGridColumns_ShouldReturnCorrectNumberOfColumns()
    {
        var grid = new[]
        {
            new[] { CellState.Dead, CellState.Alive },
            new[] { CellState.Alive, CellState.Dead }
        };
        var state = BoardState.Create(grid);

        Assert.Equal(2, state.GetGridColumns());
    }
}
