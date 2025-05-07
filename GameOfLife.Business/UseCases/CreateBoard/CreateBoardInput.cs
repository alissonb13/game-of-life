namespace GameOfLife.Business.UseCases.CreateBoard;

public class CreateBoardInput
{
    public int[][] Grid { get; private set; }

    private CreateBoardInput(int[][] grid)
    {
        Grid = grid;
    }

    public static CreateBoardInput Create(int[][] grid)
    {
        return new CreateBoardInput(grid);
    }
}