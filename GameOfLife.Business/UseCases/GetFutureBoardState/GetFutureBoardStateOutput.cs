using GameOfLife.Business.Domain.Entities;

namespace GameOfLife.Business.UseCases.GetFutureBoardState;

public record GetFutureBoardStateOutput(Guid BoardId, BoardState State);