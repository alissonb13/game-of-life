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
    IGetLatestBoardState getLatestBoardState) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadBoard([FromBody] CreateBoardRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var result = await createBoard.Execute(request.ToInput());
            return StatusCode(StatusCodes.Status201Created, new CreateBoardResponse(result.Board.Id));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{boardId:guid}/states/latest")]
    public async Task<IActionResult> GetLatestBoardState([FromRoute] Guid boardId, [FromQuery] int generationMaxValue)
    {
        if (boardId == Guid.Empty)
        {
            return BadRequest(new { message = "ID de tabuleiro inválido." });
        }

        try
        {
            return Ok(await getLatestBoardState.Execute(new GetLatestBoardStateInput(boardId, generationMaxValue)));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("{boardId:guid}/states/next")]
    public async Task<IActionResult> GetNextBoardState([FromRoute] Guid boardId)
    {
        if (boardId == Guid.Empty)
        {
            return BadRequest(new { message = "ID de tabuleiro inválido." });
        }

        try
        {
            return Ok(await getNextBoardState.Execute(new GetNextBoardStateInput(boardId)));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("{boardId:guid}/states/{generations:int}")]
    public async Task<IActionResult> GetFutureBoardState([FromRoute] Guid boardId, int generations)
    {
        if (boardId == Guid.Empty)
        {
            return BadRequest(new { message = "ID de tabuleiro inválido." });
        }

        try
        {
            return Ok(await getFutureBoardState.Execute(
                new GetFutureBoardStateInput(boardId, generations)
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}