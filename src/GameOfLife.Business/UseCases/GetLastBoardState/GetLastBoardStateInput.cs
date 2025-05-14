namespace GameOfLife.Business.UseCases.GetLastBoardState;

public record GetLastBoardStateInput(Guid BoardId, int GenerationMaxValue);