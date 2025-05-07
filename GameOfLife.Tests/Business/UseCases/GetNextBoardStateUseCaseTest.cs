using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.UseCases.GetNextBoardState;
using Moq;

namespace GameOfLife.Tests.Business.UseCases;

public class GetNextBoardStateUseCaseTest
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly GetNextBoardStateUseCase _useCase;

    public GetNextBoardStateUseCaseTest()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _useCase = new GetNextBoardStateUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldReturnNextStateAndUpdateBoard()
    {
        var boardId = Guid.NewGuid();
        var initialGrid = new[] { new[] { CellState.Dead, CellState.Alive, CellState.Dead } };
        var initialState = BoardState.Create(initialGrid);
        var board = Board.Create(initialState);

        typeof(Board).GetProperty(nameof(Board.Id))!
            .SetValue(board, boardId);

        _repositoryMock.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync(board);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Board>())).Returns(Task.CompletedTask);

        var input = new GetNextBoardStateInput(boardId);

        var output = await _useCase.Execute(input);

        Assert.Equal(boardId, output.Id);
        Assert.Equal(board.CurrentState, output.State);
        Assert.Equal(2, board.History.Count);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Board>(b => b.Id == boardId)), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowBoardNotFoundException_WhenBoardDoesNotExist()
    {
        var boardId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync((Board?)null);

        var input = new GetNextBoardStateInput(boardId);

        await Assert.ThrowsAsync<BoardNotFoundException>(() => _useCase.Execute(input));
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenInputIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _useCase.Execute(null!));
    }
}