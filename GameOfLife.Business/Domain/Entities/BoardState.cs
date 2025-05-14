using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Exceptions;

namespace GameOfLife.Business.Domain.Entities;

public class BoardState(CellState[][] grid, int generation)
{
    private const int GenerationValueMin = 1;

    public CellState[][] Grid { get; } = grid ?? throw new ArgumentNullException(nameof(grid));
    public int Generation { get; } = generation;

    public int GetGridRows() => Grid.Length;
    public int GetGridColumns() => Grid[0].Length;

    public static BoardState Create(CellState[][] grid, int generation = GenerationValueMin)
    {
        if (generation < GenerationValueMin)
        {
            throw new InvalidGenerationException(generation, GenerationValueMin);
        }

        return new BoardState(grid, generation);
    }
}