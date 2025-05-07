using GameOfLife.Business.Domain.Enums;

namespace GameOfLife.Business.Domain.Entities;

public class Board(Guid id, int rows, int columns, List<BoardState> history)
{
    public Guid Id { get; private set; } = id;
    public int Rows { get; private set; } = rows;
    public int Columns { get; private set; } = columns;
    public List<BoardState> History { get; } = history;

    public static Board Create(BoardState initialState)
    {
        ArgumentNullException.ThrowIfNull(initialState);

        return new Board(
            Guid.NewGuid(),
            initialState.GetGridRows(),
            initialState.GetGridColumns(),
            [initialState]
        );
    }

    public BoardState CurrentState => History.Last();

    public void AddState(BoardState state) => History.Add(state);

    public bool IsConcluded()
    {
        return IsExtinct() || IsStable() || IsOscillating();
    }

    private bool IsExtinct()
    {
        return CurrentState.Grid.All(row => row.All(cell => cell == CellState.Dead));
    }

    private bool IsStable()
    {
        if (History.Count < 2) return false;

        var previousState = History[^2];
        return AreGridsEqual(CurrentState.Grid, previousState.Grid);
    }

    private bool IsOscillating()
    {
        const int generationMinValue = 4;
        
        if (History.Count < generationMinValue) return false; 

        for (var i = 0; i < History.Count - 2; i++)
        {
            if (AreGridsEqual(History[i].Grid, CurrentState.Grid))
                return true;
        }

        return false;
    }

    private static bool AreGridsEqual(CellState[][] gridA, CellState[][] gridB)
    {
        if (gridA.Length != gridB.Length) return false;

        for (var i = 0; i < gridA.Length; i++)
        {
            if (gridA[i].Length != gridB[i].Length) return false;

            for (var j = 0; j < gridA[i].Length; j++)
            {
                if (gridA[i][j] != gridB[i][j])
                    return false;
            }
        }

        return true;
    }
}