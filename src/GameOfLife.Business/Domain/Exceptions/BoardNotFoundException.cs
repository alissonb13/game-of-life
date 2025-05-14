namespace GameOfLife.Business.Domain.Exceptions;

public class BoardNotFoundException : BusinessException
{
    private new const string Message = "Board {0} not found";

    public BoardNotFoundException(Guid id) : base(GetExceptionMessage(id))
    {
    }

    public BoardNotFoundException(Guid id, Exception innerException) : base(GetExceptionMessage(id), innerException)
    {
    }

    private static string GetExceptionMessage(Guid id) => string.Format(Message, id);
}