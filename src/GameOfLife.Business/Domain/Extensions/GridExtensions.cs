using GameOfLife.Business.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Business.Domain.Extensions;

public static class GridExtensions
{
    /// <summary>
    /// Converts a 2D integer array into a 2D CellState array
    /// </summary>
    /// <param name="grid">2D integer array representing the cell states</param>
    /// <returns>2D array of CellState</returns>
    /// <exception cref="ArgumentNullException">Thrown if the grid is null</exception>
    /// <exception cref="ArgumentException">Thrown if the grid is irregular or contains invalid values</exception>
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