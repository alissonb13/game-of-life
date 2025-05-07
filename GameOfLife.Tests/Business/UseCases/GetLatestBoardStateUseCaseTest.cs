using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Exceptions;
using GameOfLife.Business.Domain.Extensions;
using GameOfLife.Business.Domain.Interfaces;
using GameOfLife.Business.UseCases.GetLastBoardState;
using Moq;

namespace GameOfLife.Tests.Business.UseCases;

public class GetLatestBoardStateUseCaseTest
{
    private readonly Mock<IBoardRepository> _repositoryMock = new();

    private readonly GetLatestBoardStateUseCase _useCase;

    public GetLatestBoardStateUseCaseTest()
    {
        _useCase = new GetLatestBoardStateUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task Execute_BoardNotFound_ThrowsBoardNotFoundException()
    {
        var boardId = Guid.NewGuid();
        var input = new GetLatestBoardStateInput(boardId, 10);

        _repositoryMock.Setup(r => r.GetByIdAsync(boardId)).ReturnsAsync((Board?)null);

        await Assert.ThrowsAsync<BoardNotFoundException>(() => _useCase.Execute(input));
    }

    [Fact]
    public async Task Execute_StopsWhenIsConcluded_ReturnsEarly()
    {
        const int generationMaxValue = 5;
        var state = BoardState.Create(new int[][] { [0, 1, 0], [1, 1, 1], [0, 1, 0] }.ToCellState());
        var board = Board.Create(state);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id))
            .ReturnsAsync(board);

        var input = new GetLatestBoardStateInput(board.Id, generationMaxValue);

        var output = await _useCase.Execute(input);

        _repositoryMock.Verify(r => r.UpdateAsync(board), Times.Once);

        Assert.NotNull(output);
        Assert.Equal(board, output.Board);
    }

    [Fact]
    public async Task Execute_ReachesGenerationMaxValue_WhenNotConcluded()
    {
        const int generationMaxValue = 5;
        var state = BoardState.Create(new int[][] { [1, 0, 1], [0, 1, 0], [1, 0, 1] }.ToCellState());
        var board = Board.Create(state);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id))
            .ReturnsAsync(board);

        var input = new GetLatestBoardStateInput(board.Id, generationMaxValue);

        var output = await _useCase.Execute(input);

        _repositoryMock.Verify(r => r.UpdateAsync(board), Times.Once);

        Assert.NotNull(output);
    }
}