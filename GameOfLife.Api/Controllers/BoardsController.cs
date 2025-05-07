using GameOfLife.Api.DTOs.Requests;
using GameOfLife.Api.DTOs.Responses;
using GameOfLife.Business.UseCases.CreateBoard;
using GameOfLife.Business.UseCases.GetFutureBoardState;
using GameOfLife.Business.UseCases.GetLastBoardState;
using GameOfLife.Business.UseCases.GetNextBoardState;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameOfLife.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BoardsController(
    ICreateBoard createBoard,
    IGetNextBoardState getNextBoardState,
    IGetFutureBoardState getFutureBoardState,
    IGetLatestBoardState getLatestBoardState,
    ILogger<BoardsController> logger) : ControllerBase
{
    private readonly string InvalidBoardIdErrorMessage = "Invalid board id";
    
    [HttpPost]
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

    [HttpGet("{boardId:guid}/states/latest")]
    public async Task<IActionResult> GetLatestBoardState([FromRoute] Guid boardId, [FromQuery] int generationMaxValue)
    {
        if (boardId != Guid.Empty)
            return Ok(await getLatestBoardState.Execute(new GetLatestBoardStateInput(boardId, generationMaxValue)));

        logger.LogError("Attempt to retrieve latest board state with empty board ID.");
        return BadRequest(new { message = InvalidBoardIdErrorMessage });
    }

    [HttpGet("{boardId:guid}/states/next")]
    public async Task<IActionResult> GetNextBoardState([FromRoute] Guid boardId)
    {
        if (boardId != Guid.Empty)
            return Ok(await getNextBoardState.Execute(new GetNextBoardStateInput(boardId)));

        logger.LogError("Attempt to retrieve next board state with empty board ID.");
        return BadRequest(new { message = InvalidBoardIdErrorMessage });
    }

    [HttpGet("{boardId:guid}/states/{generations:int}")]
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