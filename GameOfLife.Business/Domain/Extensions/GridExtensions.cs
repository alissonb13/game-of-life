using GameOfLife.Business.Domain.Enums;

namespace GameOfLife.Business.Domain.Extensions;

public static class GridExtensions
{
    public static CellState[][] ToCellState(this int[][] grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        var rows = grid.Length;
        var cols = grid[0].Length;
        
        var result = new CellState[rows][];

        for (var i = 0; i < rows; i++)
        {
            result[i] = new CellState[cols];

            for (var j = 0; j < cols; j++)
            {
                var value = grid[i][j];

                if (!Enum.IsDefined(typeof(CellState), value))
                    throw new ArgumentException($"Invalid cell value at ({i},{j}): {value}");

                result[i][j] = (CellState)value;
            }
        }

        return result;
    }
}