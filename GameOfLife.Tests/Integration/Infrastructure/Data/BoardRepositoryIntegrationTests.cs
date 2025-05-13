using AutoFixture;
using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Tests.Integration.Infrastructure.Data;

public class BoardRepositoryIntegrationTests
{
    private readonly IFixture _fixture = new Fixture();

    private static GameOfLifeContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<GameOfLifeContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new GameOfLifeContext(options);
    }

    private static BoardRepository CreateRepository(GameOfLifeContext context)
    {
        return new BoardRepository(context);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBoard_WhenBoardExists()
    {
        // Arrange
        var context = CreateInMemoryContext(nameof(GetByIdAsync_ReturnsBoard_WhenBoardExists));
        var repository = CreateRepository(context);

        var boardState = BoardState.Create(_fixture.Create<CellState[][]>());
        var board = Board.Create(boardState);

        context.Boards.Add(board);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(board.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(board.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenBoardDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryContext(nameof(GetByIdAsync_ReturnsNull_WhenBoardDoesNotExist));
        var repository = CreateRepository(context);

        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_PersistsNewBoard()
    {
        // Arrange
        var context = CreateInMemoryContext(nameof(SaveAsync_PersistsNewBoard));
        var repository = CreateRepository(context);

        var boardState = BoardState.Create(_fixture.Create<CellState[][]>());
        var board = Board.Create(boardState);

        // Act
        await repository.SaveAsync(board);

        // Assert
        var saved = await context.Boards.FindAsync(board.Id);
        Assert.NotNull(saved);
        Assert.Equal(board.Id, saved?.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingBoard()
    {
        // Arrange
        var context = CreateInMemoryContext(nameof(UpdateAsync_UpdatesExistingBoard));
        var repository = CreateRepository(context);

        var boardState = BoardState.Create(_fixture.Create<CellState[][]>());
        var board = Board.Create(boardState);

        context.Boards.Add(board);
        await context.SaveChangesAsync();

        var newState = BoardState.Create(_fixture.Create<CellState[][]>());
        board.AddState(newState);

        // Act
        await repository.UpdateAsync(board);

        // Assert
        var updated = await context.Boards.FindAsync(board.Id);
        Assert.NotNull(updated);
        Assert.Equal(board.Id, updated?.Id);
        Assert.Contains(updated!.History, s => s.Generation == newState.Generation);
    }
}