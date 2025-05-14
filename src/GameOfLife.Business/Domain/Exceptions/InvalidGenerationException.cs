namespace GameOfLife.Business.Domain.Exceptions;

public class InvalidGenerationException : BusinessException
{
    private new const string Message = "Generation value {0} is invalid. It must be greater than or equal to {1}.";

    public InvalidGenerationException(int generationValue, int minGenerationValue)
        : base(GetExceptionMessage(generationValue, minGenerationValue))
    {
    }

    public InvalidGenerationException(int generationValue, int minGenerationValue, Exception innerException)
        : base(GetExceptionMessage(generationValue, minGenerationValue), innerException)
    {
    }

    private static string GetExceptionMessage(int generationValue, int minGenerationValue)
    {
        return string.Format(Message, generationValue, minGenerationValue);
    }
}