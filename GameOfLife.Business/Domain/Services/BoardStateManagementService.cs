using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Interfaces;

namespace GameOfLife.Business.Domain.Services;

public class BoardStateManagementService : IBoardStateManagementService
{
    private const int SurvivalMin = 2;
    private const int SurvivalMax = 3;
    private const int ReproductionThreshold = 3;

    public BoardState GetNextState(BoardState currentState)
    {
        var rows = currentState.GetGridRows();
        var cols = currentState.GetGridColumns();
        var nextState = new CellState[rows][];

        for (var row = 0; row < rows; row++)
        {
            nextState[row] = new CellState[cols];
            for (var col = 0; col < cols; col++)
            {
                nextState[row][col] = CalculateNextCellState(currentState, row, col);
            }
        }

        return new BoardState(nextState, currentState.Generation + 1);
    }

    private static CellState CalculateNextCellState(BoardState currentState, int row, int col)
    {
        var isAlive = currentState.Grid[row][col] == CellState.Alive;
        var neighbors = CountLiveNeighbors(currentState, row, col);

        var shouldRevive = !isAlive && neighbors == ReproductionThreshold;
        var shouldSurvive = isAlive && (neighbors == SurvivalMin || neighbors == SurvivalMax);

        return shouldRevive || shouldSurvive ? CellState.Alive : CellState.Dead;
    }

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

                var isValidPosition = IsValidPosition(neighborRow, neighborCol, rows, cols);
                var isNeighborAlive = currentState.Grid[neighborRow][neighborCol] == CellState.Alive;

                if (isValidPosition && isNeighborAlive)
                {
                    liveNeighbors++;
                }
            }
        }

        return liveNeighbors;
    }

    private static bool IsValidPosition(int row, int col, int rows, int cols)
    {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }
}