using AutoFixture;
using GameOfLife.Business.Domain.Entities;
using GameOfLife.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Tests.Infrastructure.Data;

public class BoardRepositoryTest
{
    private readonly IFixture _fixture;
    private readonly BoardRepository _repository;
    private readonly GameOfLifeContext _context;

    public BoardRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<GameOfLifeContext>()
            .UseInMemoryDatabase(databaseName: "GameOfLifeTestDb")
            .Options;

        _fixture = new Fixture();
        _context = new GameOfLifeContext(options);
        _repository = new BoardRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBoard_WhenBoardExists()
    {
        var initialState = _fixture.Create<BoardState>();
        var board = Board.Create(initialState); 

        _context.Boards.Add(board);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(board.Id);

        Assert.NotNull(result);
        Assert.Equal(result.Id, board.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBoardDoesNotExist()
    {
        var boardId = Guid.NewGuid();

        var result = await _repository.GetByIdAsync(boardId);

        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_ShouldAddBoard_WhenBoardIsNew()
    {
        var board = _fixture.Create<Board>();

        await _repository.SaveAsync(board);

        var savedBoard = await _context.Boards.FindAsync(board.Id);
        Assert.NotNull(savedBoard);
        Assert.Equal(board.Id, savedBoard?.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBoard_WhenBoardExists()
    {
        var board = _fixture.Create<Board>();
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();

        await _repository.UpdateAsync(board);

        var updatedBoard = await _context.Boards.FindAsync(board.Id);
        Assert.NotNull(updatedBoard);
    }
}