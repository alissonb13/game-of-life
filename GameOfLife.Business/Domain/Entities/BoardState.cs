using GameOfLife.Business.Domain.Enums;

namespace GameOfLife.Business.Domain.Entities;

public class BoardState(CellState[][] grid, int generation)
{
    private const int SurvivalMin = 2;
    private const int SurvivalMax = 3;
    private const int GenerationValueMin = 1;
    private const int ReproductionThreshold = 3;

    public CellState[][] Grid { get; } = grid ?? throw new ArgumentNullException(nameof(grid));
    public int Generation { get; } = generation;

    public int GetGridRows() => Grid.Length;
    public int GetGridColumns() => Grid[0].Length;

    public static BoardState Create(CellState[][] grid, int generation = GenerationValueMin)
    {
        return new BoardState(grid, generation);
    }

    public BoardState GetNextState()
    {
        var rows = GetGridRows();
        var cols = GetGridColumns();
        var nextState = new CellState[rows][];

        for (var row = 0; row < rows; row++)
        {
            nextState[row] = new CellState[cols];
            for (var col = 0; col < cols; col++)
            {
                nextState[row][col] = CalculateNextCellState(row, col);
            }
        }

        return new BoardState(nextState, Generation + 1);
    }

    private CellState CalculateNextCellState(int row, int col)
    {
        var isAlive = Grid[row][col] == CellState.Alive;
        var neighbors = CountLiveNeighbors(row, col);

        var shouldRevive = !isAlive && neighbors == ReproductionThreshold;
        var shouldSurvive = isAlive && neighbors is SurvivalMin or SurvivalMax;

        return shouldRevive || shouldSurvive ? CellState.Alive : CellState.Dead;
    }

    private int CountLiveNeighbors(int row, int col)
    {
        var liveNeighbors = 0;

        for (var deltaRow = -1; deltaRow <= 1; deltaRow++)
        {
            for (var deltaCol = -1; deltaCol <= 1; deltaCol++)
            {
                if (deltaRow == 0 && deltaCol == 0) continue;

                var neighborRow = row + deltaRow;
                var neighborCol = col + deltaCol;

                if (IsValidPosition(neighborRow, neighborCol) && Grid[neighborRow][neighborCol] == CellState.Alive)
                {
                    liveNeighbors++;
                }
            }
        }

        return liveNeighbors;
    }

    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < GetGridRows() && col >= 0 && col < GetGridColumns();
    }
}