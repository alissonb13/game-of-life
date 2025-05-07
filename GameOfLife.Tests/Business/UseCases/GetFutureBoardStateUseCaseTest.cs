using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.UseCases.GetFutureBoardState;
using Moq;

namespace GameOfLife.Tests.Business.UseCases;

public class GetFutureBoardStateUseCaseTest
{
    private readonly Mock<IBoardRepository> _repositoryMock = new();
    private readonly GetFutureBoardStateUseCase _useCase;

    public GetFutureBoardStateUseCaseTest()
    {
        _useCase = new GetFutureBoardStateUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidFutureStateException_WhenFutureStatesIsLessThanOne()
    {
        var input = new GetFutureBoardStateInput(Guid.NewGuid(), 0);

        await Assert.ThrowsAsync<InvalidFutureStateException>(() => _useCase.Execute(input));
    }

    [Fact]
    public async Task Execute_ShouldThrowBoardNotFoundException_WhenBoardIsNotFound()
    {
        var boardId = Guid.NewGuid();
        var input = new GetFutureBoardStateInput(boardId, 2);

        _repositoryMock.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync((Board?)null);

        await Assert.ThrowsAsync<BoardNotFoundException>(() => _useCase.Execute(input));
    }

    [Fact]
    public async Task Execute_ShouldAddFutureStatesAndUpdateBoard()
    {
        var initialGrid = new[] { new[] { CellState.Dead, CellState.Alive, CellState.Dead } };
        var initialState = BoardState.Create(initialGrid);
        var board = Board.Create(initialState);
        var boardId = board.Id;

        var input = new GetFutureBoardStateInput(boardId, 2);

        _repositoryMock.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync(board);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Board>())).Returns(Task.CompletedTask);

        var output = await _useCase.Execute(input);

        Assert.Equal(board.Id, output.BoardId);
        Assert.Equal(initialState.Generation + 2, output.State.Generation);
        Assert.Equal(3, board.History.Count);
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Board>(b => b.Id == boardId)), Times.Once);
    }
}