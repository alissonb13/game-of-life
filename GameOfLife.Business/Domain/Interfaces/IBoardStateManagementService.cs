using GameOfLife.Business.Domain.Entities;

namespace GameOfLife.Business.Domain.Interfaces;

public interface IBoardStateManagementService
{
    BoardState GetNextState(BoardState currentState);
}