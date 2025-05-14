using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Enums;
using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.UseCases.GetNextBoardState;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameOfLife.Tests.Unit.Business.UseCases;

public class GetNextBoardStateUseCaseUnitTests
{
    private readonly Mock<IBoardService> _boardServiceMock;
    private readonly Mock<IBoardStateManagementService> _boardStateManagementServiceMock;

    private readonly GetNextBoardStateUseCase _useCase;

    public GetNextBoardStateUseCaseUnitTests()
    {
        _boardServiceMock = new Mock<IBoardService>();
        _boardStateManagementServiceMock = new Mock<IBoardStateManagementService>();

        var loggerMock = new Mock<ILogger<GetNextBoardStateUseCase>>();

        _useCase = new GetNextBoardStateUseCase(
            _boardServiceMock.Object,
            _boardStateManagementServiceMock.Object,
            loggerMock.Object
        );
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

        _boardServiceMock.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync(board);
        _boardServiceMock.Setup(r => r.UpdateAsync(It.IsAny<Board>())).Returns(Task.CompletedTask);

        var nextGrid = new[] { new[] { CellState.Alive, CellState.Alive, CellState.Dead } };
        var nextState = BoardState.Create(nextGrid);
        _boardStateManagementServiceMock
            .Setup(s => s.GetNextState(It.IsAny<BoardState>()))
            .Returns(nextState);

        var input = new GetNextBoardStateInput(boardId);
        var output = await _useCase.Execute(input);

        Assert.Equal(boardId, output.Id);
        Assert.Equal(board.CurrentState, output.State);
        Assert.Equal(2, board.History.Count);

        _boardServiceMock.Verify(r => r.UpdateAsync(It.Is<Board>(b => b.Id == boardId)), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowBoardNotFoundException_WhenBoardDoesNotExist()
    {
        var boardId = Guid.NewGuid();
        
        _boardServiceMock
            .Setup(r => r.GetByIdAsync(boardId))
            .ThrowsAsync(new BoardNotFoundException(boardId));

        var input = new GetNextBoardStateInput(boardId);

        await Assert.ThrowsAsync<BoardNotFoundException>(() => _useCase.Execute(input));
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenInputIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _useCase.Execute(null!));
    }
}