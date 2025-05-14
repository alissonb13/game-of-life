using AutoFixture;
using GameOfLife.Api.Controllers;
using GameOfLife.Api.DTOs.Requests;
using GameOfLife.Api.DTOs.Responses;
using GameOfLife.Business.Domain.Entities;
using GameOfLife.Business.Domain.Extensions;
using GameOfLife.Business.UseCases.CreateBoard;
using GameOfLife.Business.UseCases.GetFutureBoardState;
using GameOfLife.Business.UseCases.GetLastBoardState;
using GameOfLife.Business.UseCases.GetNextBoardState;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameOfLife.Tests.Unit.Api.Controllers;

public class BoardsControllerTest
{
    private readonly Fixture _fixture;

    private readonly Mock<ICreateBoard> _createBoardMock = new();
    private readonly Mock<IGetNextBoardState> _getNextBoardStateMock = new();
    private readonly Mock<IGetFutureBoardState> _getFutureBoardStateMock = new();
    private readonly Mock<IGetLastBoardState> _getLatestBoardStateMock = new();
    private readonly Mock<ILogger<BoardsController>> _loggerMock = new();

    private readonly BoardsController _controller;

    public BoardsControllerTest()
    {
        _fixture = new Fixture();
        _controller = new BoardsController(
            _createBoardMock.Object,
            _getNextBoardStateMock.Object,
            _getFutureBoardStateMock.Object,
            _getLatestBoardStateMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task UploadBoard_ValidRequest_ReturnsCreated()
    {
        var request = new CreateBoardRequest
        {
            Grid = new int[][] { [0, 1, 0], [1, 1, 1], [0, 1, 0] }
        };

        var expectedBoard = Board.Create(BoardState.Create(request.ToInput().Grid.ToCellState()));
        var expectedOutput = new CreateBoardOutput(expectedBoard);

        _createBoardMock
            .Setup(x => x.Execute(It.IsAny<CreateBoardInput>()))
            .ReturnsAsync(expectedOutput);

        var result = await _controller.UploadBoard(request);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);

        var response = Assert.IsType<CreateBoardResponse>(objectResult.Value);
        Assert.Equal(expectedBoard.Id, response.Id);

        _createBoardMock.Verify(
            x => x.Execute(It.IsAny<CreateBoardInput>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UploadBoard_InvalidModel_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("Grid", "O grid é obrigatório");

        var request = new CreateBoardRequest { Grid = null! };

        var result = await _controller.UploadBoard(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task GetNextBoardState_ReturnsOk_WhenSuccessful()
    {
        var grid = new int[][]
        {
            [0, 1, 0],
            [1, 1, 1],
            [0, 1, 0]
        };
        var boardId = Guid.NewGuid();
        var boardState = BoardState.Create(grid.ToCellState());
        var expectedOutput = new GetNextBoardStateOutput(boardId, boardState);

        _getNextBoardStateMock
            .Setup(g => g.Execute(It.Is<GetNextBoardStateInput>(input => input.Id == boardId)))
            .ReturnsAsync(expectedOutput);

        var result = await _controller.GetNextBoardState(boardId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedOutput, okResult.Value);
    }

    [Fact]
    public async Task GetNextBoardState_ReturnsBadRequest_WhenIdIsEmpty()
    {
        var result = await _controller.GetNextBoardState(Guid.Empty);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetFutureBoardState_ReturnsOk_WhenSuccessful()
    {
        var grid = new int[][]
        {
            [0, 1, 0],
            [1, 1, 1],
            [0, 1, 0]
        };
        var input = _fixture.Build<GetFutureBoardStateInput>()
            .With(x => x.Id, Guid.NewGuid())
            .With(x => x.FutureStates, _fixture.Create<int>())
            .Create();
        var boardState = BoardState.Create(grid.ToCellState());
        var expectedOutput = new GetFutureBoardStateOutput(input.Id, boardState);

        _getFutureBoardStateMock
            .Setup(g => g.Execute(input))
            .ReturnsAsync(expectedOutput);

        var result = await _controller.GetFutureBoardState(input.Id, input.FutureStates);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedOutput, okResult.Value);
    }

    [Fact]
    public async Task GetFutureBoardState_ReturnsBadRequest_WhenIdIsEmpty()
    {
        var result = await _controller.GetFutureBoardState(Guid.Empty, 3);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetLatestBoardState_ReturnsBadRequest_WhenBoardIdIsEmpty()
    {
        const int generationMaxValue = 10;

        var result = await _controller.GetLatestBoardState(Guid.Empty, generationMaxValue);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetLatestBoardState_ReturnsOk_WhenSuccessful()
    {
        var boardId = Guid.NewGuid();
        const int generationMaxValue = 5;
        var expectedState = BoardState.Create(new int[][] { [0, 1, 0], [1, 1, 1], [0, 1, 0] }.ToCellState());
        var expectedBoard = Board.Create(expectedState);
        var expectedOutput = new GetLastBoardStateOutput(expectedBoard);

        _getLatestBoardStateMock
            .Setup(x => x.Execute(It.Is<GetLastBoardStateInput>(input =>
                input.BoardId == boardId && input.GenerationMaxValue == generationMaxValue)))
            .ReturnsAsync(expectedOutput);

        var result = await _controller.GetLatestBoardState(boardId, generationMaxValue);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedOutput, okResult.Value);
    }
}