using AutoFixture;
using GameOfLife.Business.Domain.Enums;

namespace GameOfLife.Tests.Helpers;

public static class GridHelper
{
    public static CellState[][] CreateGrid(int rows, int cols)
    {
        var fixture = new Fixture();

        return Enumerable.Range(0, rows)
            .Select(_ => Enumerable
                .Repeat(fixture.Create<CellState>(), cols)
                .ToArray()
            )
            .ToArray();
    }
}