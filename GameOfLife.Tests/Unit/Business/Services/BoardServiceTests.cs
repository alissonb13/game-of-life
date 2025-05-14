using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.Domain.Services;
using GameOfLife.Tests.Helpers;
using Moq;

namespace GameOfLife.Tests.Unit.Business.Services;

public class BoardServiceTests
{
    private readonly Mock<ICacheProvider> _cacheProviderMock;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly BoardService _service;

    public BoardServiceTests()
    {
        _cacheProviderMock = new Mock<ICacheProvider>();
        _boardRepositoryMock = new Mock<IBoardRepository>();

        _service = new BoardService(
            _cacheProviderMock.Object,
            _boardRepositoryMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBoard_FromCache()
    {
        var boardId = Guid.NewGuid();
        var grids = Enumerable.Range(0, 5)
            .Select(_ => GridHelper.CreateGrid(3, 3))
            .ToList();
        var boardState = BoardState.Create(grids.First());
        var cachedBoard = Board.Create(boardState);
        _cacheProviderMock.Setup(c => c.Get<Board>(boardId.ToString())).Returns(cachedBoard);

        var result = await _service.GetByIdAsync(boardId);

        Assert.Equal(cachedBoard, result);
        _boardRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBoard_FromRepository_WhenNotInCache()
    {
        var boardId = Guid.NewGuid();
        var grids = Enumerable.Range(0, 5)
            .Select(_ => GridHelper.CreateGrid(3, 3))
            .ToList();
        var boardState = BoardState.Create(grids.First());
        var boardFromDb = Board.Create(boardState);

        _cacheProviderMock.Setup(c => c.Get<Board>(boardId.ToString())).Returns((Board?)null);
        _boardRepositoryMock.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync(boardFromDb);

        var result = await _service.GetByIdAsync(boardId);

        Assert.Equal(boardFromDb, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
    {
        var boardId = Guid.NewGuid();
        _cacheProviderMock.Setup(c => c.Get<Board>(boardId.ToString())).Returns((Board?)null);
        _boardRepositoryMock.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync((Board?)null);

        await Assert.ThrowsAsync<BoardNotFoundException>(() => _service.GetByIdAsync(boardId));
    }

    [Fact]
    public async Task CreateAsync_ShouldSaveToRepository_AndCache()
    {
        var grids = Enumerable.Range(0, 5)
            .Select(_ => GridHelper.CreateGrid(3, 3))
            .ToList();
        var boardState = BoardState.Create(grids.First());
        var board = Board.Create(boardState);

        await _service.CreateAsync(board);

        _boardRepositoryMock.Verify(r => r.SaveAsync(board), Times.Once);
        _cacheProviderMock.Verify(c => c.Set(board.Id.ToString(), board), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateRepository_AndCache()
    {
        var grids = Enumerable.Range(0, 5)
            .Select(_ => GridHelper.CreateGrid(3, 3))
            .ToList();
        var boardState = BoardState.Create(grids.First());
        var board = Board.Create(boardState);

        await _service.UpdateAsync(board);

        _boardRepositoryMock.Verify(r => r.UpdateAsync(board), Times.Once);
        _cacheProviderMock.Verify(c => c.Set(board.Id.ToString(), board), Times.Once);
    }

    [Fact]
    public void GetExistingStateFromBoardByGeneration_ShouldReturnCorrectState()
    {
        var grids = Enumerable.Range(0, 5)
            .Select(_ => GridHelper.CreateGrid(3, 3))
            .ToList();
        var boardState = BoardState.Create(grids.First());
        var board = Board.Create(boardState);

        var result = _service.GetExistingStateFromBoardByGeneration(board, 1);

        Assert.NotNull(result);
        Assert.Single(result!.History);
        Assert.Equal(1, result.History[0].Generation);
    }

    [Fact]
    public void GetExistingStateFromBoardByGeneration_ShouldReturnNull_WhenNotFound()
    {
        var board = new Board(Guid.NewGuid(), 2, 2, []);

        var result = _service.GetExistingStateFromBoardByGeneration(board, 10);

        Assert.Null(result);
    }
}