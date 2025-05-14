namespace GameOfLife.Business.Domain.Exceptions;

public class InvalidFutureStateException : BusinessException
{
    private new const string Message = "Future state {0} is invalid";

    public InvalidFutureStateException(int futureState)
        : base(GetExceptionMessage(futureState))
    {
    }

    public InvalidFutureStateException(int futureState, Exception innerException)
        : base(GetExceptionMessage(futureState), innerException)
    {
    }

    private static string GetExceptionMessage(int futureState) => string.Format(Message, futureState);
}