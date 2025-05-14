using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Services;

namespace GameOfLife.Tests.Unit.Business.Services;

public class BoardStateManagementServiceUnitTests
{
    private readonly BoardStateManagementService _boardStateManagementService = new();

    [Fact]
    public void GetNextState_ShouldReturnNextGenerationWithCorrectState()
    {
        var initialGrid = new CellState[][]
        {
            [CellState.Dead, CellState.Alive, CellState.Dead],
            [CellState.Alive, CellState.Alive, CellState.Dead],
            [CellState.Dead, CellState.Dead, CellState.Dead]
        };
        var initialState = new BoardState(initialGrid, 0);

        var nextState = _boardStateManagementService.GetNextState(initialState);

        Assert.Equal(1, nextState.Generation); 
        Assert.Equal(CellState.Alive, nextState.Grid[0][1]); 
        Assert.Equal(CellState.Alive, nextState.Grid[1][1]);
        Assert.Equal(CellState.Dead, nextState.Grid[2][0]); 
    }

    [Fact]
    public void GetNextState_ShouldReviveCellWithExactlyThreeNeighbors()
    {
        var initialGrid = new CellState[][]
        {
            [CellState.Dead, CellState.Alive, CellState.Dead],
            [CellState.Dead, CellState.Dead, CellState.Alive],
            [CellState.Dead, CellState.Alive, CellState.Dead]
        };
        var initialState = new BoardState(initialGrid, 0);

        var nextState = _boardStateManagementService.GetNextState(initialState);

        Assert.Equal(CellState.Alive, nextState.Grid[1][1]); 
    }

    [Fact]
    public void GetNextState_ShouldSurviveCellWithTwoOrThreeNeighbors()
    {
        var initialGrid = new CellState[][]
        {
            [CellState.Dead, CellState.Alive, CellState.Dead],
            [CellState.Alive, CellState.Alive, CellState.Alive],
            [CellState.Dead, CellState.Dead, CellState.Dead]
        };
        var initialState = new BoardState(initialGrid, 0);

        var nextState = _boardStateManagementService.GetNextState(initialState);

        Assert.Equal(CellState.Alive, nextState.Grid[1][1]);
    }

    [Fact]
    public void GetNextState_ShouldKillCellWithLessThanTwoNeighbors()
    {
        var initialGrid = new CellState[][]
        {
            [CellState.Dead, CellState.Dead, CellState.Dead],
            [CellState.Alive, CellState.Dead, CellState.Dead],
            [CellState.Dead, CellState.Dead, CellState.Dead]
        };
        var initialState = new BoardState(initialGrid, 0);

        var nextState = _boardStateManagementService.GetNextState(initialState);

        Assert.Equal(CellState.Dead, nextState.Grid[1][0]);
    }

    [Fact]
    public void GetNextState_ShouldReturnDeadGrid_WhenAllCellsDie()
    {
        var initialGrid = new CellState[][]
        {
            [CellState.Dead, CellState.Dead, CellState.Dead],
            [CellState.Dead, CellState.Dead, CellState.Dead],
            [CellState.Dead, CellState.Dead, CellState.Dead]
        };
        var initialState = new BoardState(initialGrid, 0);

        var nextState = _boardStateManagementService.GetNextState(initialState);

        foreach (var row in nextState.Grid)
        {
            foreach (var cell in row)
            {
                Assert.Equal(CellState.Dead, cell);
            }
        }
    }
}