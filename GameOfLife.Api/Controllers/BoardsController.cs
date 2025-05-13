using GameOfLife.Api.DTOs.Requests;
using GameOfLife.Api.DTOs.Responses;
using GameOfLife.Business.UseCases.CreateBoard;
using GameOfLife.Business.UseCases.GetFutureBoardState;
using GameOfLife.Business.UseCases.GetLastBoardState;
using GameOfLife.Business.UseCases.GetNextBoardState;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameOfLife.Api.Controllers;

/// <summary>
/// Controller responsible for managing Game of Life boards and their states.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class BoardsController(
    ICreateBoard createBoard,
    IGetNextBoardState getNextBoardState,
    IGetFutureBoardState getFutureBoardState,
    IGetLatestBoardState getLatestBoardState,
    ILogger<BoardsController> logger) : ControllerBase
{
    private const string InvalidBoardIdErrorMessage = "Invalid board id";

    /// <summary>
    /// Creates a new board with the provided initial state.
    /// </summary>
    /// <param name="request">Initial state of the board</param>
    /// <returns>The ID of the created board</returns>
    /// <response code="201">Board created successfully</response>
    /// <response code="400">Invalid request payload</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateBoardResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadBoard([FromBody] CreateBoardRequest request)
    {
        if (!ModelState.IsValid)
        {
            logger.LogError("Invalid board creation request: {@Request}", request);
            return BadRequest(ModelState);
        }

        var result = await createBoard.Execute(request.ToInput());
        logger.LogInformation("Board created successfully with ID: {BoardId}", result.Board.Id);

        return StatusCode(StatusCodes.Status201Created, new CreateBoardResponse(result.Board.Id));
    }

    /// <summary>
    /// Gets the latest state of a board, stopping when a maximum generation count is reached or the board reaches a stable state.
    /// </summary>
    /// <param name="boardId">Board identifier</param>
    /// <param name="generationMaxValue">Maximum number of generations to calculate</param>
    /// <returns>The latest state of the board</returns>
    /// <response code="200">Latest state returned successfully</response>
    /// <response code="400">Invalid board ID</response>
    [HttpGet("{boardId:guid}/states/latest")]
    [ProducesResponseType(typeof(GetLatestBoardStateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLatestBoardState([FromRoute] Guid boardId, [FromQuery] int generationMaxValue)
    {
        if (boardId != Guid.Empty)
            return Ok(await getLatestBoardState.Execute(new GetLatestBoardStateInput(boardId, generationMaxValue)));

        logger.LogError("Attempt to retrieve latest board state with empty board ID.");
        return BadRequest(new { message = InvalidBoardIdErrorMessage });
    }

    /// <summary>
    /// Gets the next state of a board based on the current state.
    /// </summary>
    /// <param name="boardId">Board identifier</param>
    /// <returns>The next state of the board</returns>
    /// <response code="200">Next state returned successfully</response>
    /// <response code="400">Invalid board ID</response>
    [HttpGet("{boardId:guid}/states/next")]
    [ProducesResponseType(typeof(GetNextBoardStateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetNextBoardState([FromRoute] Guid boardId)
    {
        if (boardId != Guid.Empty)
            return Ok(await getNextBoardState.Execute(new GetNextBoardStateInput(boardId)));

        logger.LogError("Attempt to retrieve next board state with empty board ID.");
        return BadRequest(new { message = InvalidBoardIdErrorMessage });
    }

    /// <summary>
    /// Simulates a number of generations into the future and returns the resulting board state.
    /// </summary>
    /// <param name="boardId">Board identifier</param>
    /// <param name="generations">Number of generations to simulate</param>
    /// <returns>The final board state after simulation</returns>
    /// <response code="200">Future state calculated successfully</response>
    /// <response code="400">Invalid board ID</response>
    [HttpGet("{boardId:guid}/states/{generations:int}")]
    [ProducesResponseType(typeof(GetFutureBoardStateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFutureBoardState([FromRoute] Guid boardId, int generations)
    {
        if (boardId != Guid.Empty)
            return Ok(await getFutureBoardState.Execute(
                new GetFutureBoardStateInput(boardId, generations)
            ));

        logger.LogError("Attempt to retrieve future board state with empty board ID.");
        return BadRequest(new { message = InvalidBoardIdErrorMessage });
    }
}