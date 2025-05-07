using GameOfLife.Business.Domain.Entities;

namespace GameOfLife.Business.UseCases.GetNextBoardState;

public record GetNextBoardStateOutput(Guid Id, BoardState State)
{
    
}