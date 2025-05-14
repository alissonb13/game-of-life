using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Interfaces;

namespace GameOfLife.Business.Domain.Services;

/// <summary>
/// Service responsible for managing the evolution of the Game of Life board state.
/// </summary>
public class BoardStateManagementService : IBoardStateManagementService
{
    private const int SurvivalMin = 2;
    private const int SurvivalMax = 3;
    private const int ReproductionThreshold = 3;

    
    /// <summary>
    /// Calculates the next generation of the board based on the current state.
    /// </summary>
    /// <param name="currentState">The current state of the board.</param>
    /// <returns>The next state of the board.</returns>
    public BoardState GetNextState(BoardState currentState)
    {
        var rows = currentState.GetGridRows();
        var cols = currentState.GetGridColumns();
        var nextState = new CellState[rows][];

        Parallel.For(0, rows, row =>
        {
            nextState[row] = new CellState[cols];
            for (var col = 0; col < cols; col++)
            {
                nextState[row][col] = CalculateNextCellState(currentState, row, col);
            }
        });
        
        return new BoardState(nextState, currentState.Generation + 1);
    }
    
    /// <summary>
    /// Determines the next state of a specific cell based on the Game of Life rules.
    /// </summary>
    /// <param name="currentState">The current state of the board.</param>
    /// <param name="row">The row of the cell.</param>
    /// <param name="col">The column of the cell.</param>
    /// <returns>The next state of the cell.</returns>
    private static CellState CalculateNextCellState(BoardState currentState, int row, int col)
    {
        var isAlive = currentState.Grid[row][col] == CellState.Alive;
        var neighbors = CountLiveNeighbors(currentState, row, col);

        var shouldRevive = !isAlive && neighbors == ReproductionThreshold;
        var shouldSurvive = isAlive && (neighbors == SurvivalMin || neighbors == SurvivalMax);

        return shouldRevive || shouldSurvive ? CellState.Alive : CellState.Dead;
    }

    /// <summary>
    /// Counts the number of live neighbors around a specific cell.
    /// </summary>
    /// <param name="currentState">The current state of the board.</param>
    /// <param name="row">The row of the cell.</param>
    /// <param name="col">The column of the cell.</param>
    /// <returns>The number of live neighboring cells.</returns>
    private static int CountLiveNeighbors(BoardState currentState, int row, int col)
    {
        var liveNeighbors = 0;
        var rows = currentState.GetGridRows();
        var cols = currentState.GetGridColumns();

        for (var deltaRow = -1; deltaRow <= 1; deltaRow++)
        {
            for (var deltaCol = -1; deltaCol <= 1; deltaCol++)
            {
                if (deltaRow == 0 && deltaCol == 0) continue;

                var neighborRow = row + deltaRow;
                var neighborCol = col + deltaCol;

                if (!IsValidPosition(neighborRow, neighborCol, rows, cols)) continue;
                
                var isNeighborAlive = currentState.Grid[neighborRow][neighborCol] == CellState.Alive;
                if (isNeighborAlive)
                {
                    liveNeighbors++;
                }
            }
        }

        return liveNeighbors;
    }

    
    /// <summary>
    /// Checks if a cell position is within the bounds of the board.
    /// </summary>
    /// <param name="row">Row index to check.</param>
    /// <param name="col">Column index to check.</param>
    /// <param name="rows">Total number of rows on the board.</param>
    /// <param name="cols">Total number of columns on the board.</param>
    /// <returns>True if the position is valid; otherwise, false.</returns>
    private static bool IsValidPosition(int row, int col, int rows, int cols)
    {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }
}