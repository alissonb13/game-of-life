using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Extensions;

namespace GameOfLife.Tests.Business.Extensions;

public class GridExtensionsTest
{
    [Fact]
    public void ToCellState_ShouldConvertValidIntGridToCellStateGrid()
    {
        int[][] intGrid =
        [
            [0, 1],
            [1, 0]
        ];

        var cellStateGrid = intGrid.ToCellState();

        Assert.Equal(CellState.Dead, cellStateGrid[0][0]);
        Assert.Equal(CellState.Alive, cellStateGrid[0][1]);
        Assert.Equal(CellState.Alive, cellStateGrid[1][0]);
        Assert.Equal(CellState.Dead, cellStateGrid[1][1]);
    }

    [Fact]
    public void ToCellState_ShouldThrowArgumentNullException_WhenGridIsNull()
    {
        int[][]? grid = null;

        Assert.Throws<ArgumentNullException>(() => grid!.ToCellState());
    }

    [Fact]
    public void ToCellState_ShouldThrowArgumentException_WhenGridHasInvalidValue()
    {
        int[][] gridWithInvalidValue =
        [
            [0, 2]
        ];

        Assert.Throws<ArgumentException>(() => gridWithInvalidValue.ToCellState());
    }
}