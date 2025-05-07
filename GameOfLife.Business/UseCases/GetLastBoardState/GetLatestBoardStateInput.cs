namespace GameOfLife.Business.UseCases.GetLastBoardState;

public record GetLatestBoardStateInput(Guid BoardId, int GenerationMaxValue);