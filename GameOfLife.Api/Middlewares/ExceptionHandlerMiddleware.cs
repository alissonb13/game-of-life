using GameOfLife.Business.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Api.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException businessException)
        {
            var problem = new ProblemDetails
            {
                Title = "Business exception occured",
                Detail = businessException.Message,
                Status = StatusCodes.Status422UnprocessableEntity,
                Type = businessException.GetType().Name
            };

            context.Response.StatusCode = problem.Status.Value;
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);

            var problem = new ProblemDetails
            {
                Title = "An error occured",
                Detail = exception.Message,
                Status = StatusCodes.Status500InternalServerError,
                Type = exception.GetType().Name
            };

            context.Response.StatusCode = problem.Status.Value;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}