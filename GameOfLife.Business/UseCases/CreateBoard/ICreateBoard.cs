using GameOfLife.Business.Domain.Entities;

namespace GameOfLife.Business.UseCases.CreateBoard;

public interface ICreateBoard
{
    Task<CreateBoardOutput> Execute(CreateBoardInput input);
}