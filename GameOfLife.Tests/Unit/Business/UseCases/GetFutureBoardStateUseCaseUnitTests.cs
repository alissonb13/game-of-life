using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.UseCases.GetFutureBoardState;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameOfLife.Tests.Unit.Business.UseCases;

public class GetFutureBoardStateUseCaseUnitTests
{
    private readonly Mock<IBoardRepository> _repositoryMock = new();
    private readonly Mock<IBoardStateManagementService> _serviceMock = new();
    private readonly Mock<ILogger<GetFutureBoardStateUseCase>> _loggerMock = new();

    private readonly GetFutureBoardStateUseCase _useCase;

    public GetFutureBoardStateUseCaseUnitTests()
    {
        _useCase = new GetFutureBoardStateUseCase(
            _repositoryMock.Object,
            _serviceMock.Object,
            _loggerMock.Object
        );
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
        var nextState1 = BoardState.Create([[CellState.Alive, CellState.Alive, CellState.Dead]],
            initialState.Generation + 1);
        var nextState2 =
            BoardState.Create([[CellState.Dead, CellState.Dead, CellState.Dead]], initialState.Generation + 2);
        var board = Board.Create(initialState);
        var boardId = board.Id;

        var input = new GetFutureBoardStateInput(boardId, 2);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(boardId))
            .ReturnsAsync(board);
        _serviceMock
            .SetupSequence(s => s.GetNextState(It.IsAny<BoardState>()))
            .Returns(nextState1)
            .Returns(nextState2);
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Board>()))
            .Returns(Task.CompletedTask);

        var output = await _useCase.Execute(input);

        Assert.Equal(board.Id, output.BoardId);
        Assert.Equal(initialState.Generation + 2, output.State.Generation);
        Assert.Equal(3, board.History.Count);
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Board>(b => b.Id == boardId)), Times.Once);
    }
}