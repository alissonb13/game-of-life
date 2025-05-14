using System.ComponentModel.DataAnnotations;
using GameOfLife.Business.UseCases.CreateBoard;

namespace GameOfLife.Api.DTOs.Requests;

public record CreateBoardRequest
{
    [Required] public int[][] Grid { get; set; } = null!;

    public CreateBoardInput ToInput()
    {
        return CreateBoardInput.Create(Grid);
    }
}