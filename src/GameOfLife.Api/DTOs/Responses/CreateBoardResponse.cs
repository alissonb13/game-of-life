using GameOfLife.Business.UseCases.CreateBoard;

namespace GameOfLife.Api.DTOs.Responses;

public record CreateBoardResponse(Guid Id)
{
    public static CreateBoardResponse FromOutput(CreateBoardOutput output)
    {
        return new CreateBoardResponse(output.Board.Id);
    }   
}