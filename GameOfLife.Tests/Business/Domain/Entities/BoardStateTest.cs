using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;

namespace GameOfLife.Tests.Business.Domain.Entities;

public class BoardStateTest
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

    [Fact]
    public void GetNextState_WithReproduction_ShouldReviveDeadCell()
    {
        var grid = new[]
        {
            new[] { CellState.Dead,  CellState.Alive, CellState.Dead },
            new[] { CellState.Alive, CellState.Dead,  CellState.Alive },
            new[] { CellState.Dead,  CellState.Dead,  CellState.Dead },
        };
        var state = BoardState.Create(grid, 1);

        var nextState = state.GetNextState();

        Assert.Equal(2, nextState.Generation);
        Assert.Equal(CellState.Alive, nextState.Grid[1][1]); 
    }

    [Fact]
    public void GetNextState_WithUnderpopulation_ShouldKillCell()
    {
        var grid = new[]
        {
            new[] { CellState.Dead, CellState.Dead, CellState.Dead },
            new[] { CellState.Dead, CellState.Alive, CellState.Dead },
            new[] { CellState.Dead, CellState.Alive, CellState.Dead },
        };
        var state = BoardState.Create(grid, 1);

        var nextState = state.GetNextState();

        Assert.Equal(CellState.Dead, nextState.Grid[1][1]);
    }

    [Fact]
    public void GetNextState_WithSurvival_ShouldKeepCellAlive()
    {
        var grid = new[]
        {
            new[] { CellState.Dead,  CellState.Alive, CellState.Dead },
            new[] { CellState.Alive, CellState.Alive, CellState.Dead },
            new[] { CellState.Dead,  CellState.Dead,  CellState.Dead },
        };
        var state = BoardState.Create(grid, 3);

        var nextState = state.GetNextState();

        Assert.Equal(CellState.Alive, nextState.Grid[1][1]); 
    }
}
