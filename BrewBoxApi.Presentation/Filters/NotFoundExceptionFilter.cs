using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BrewBoxApi.Application.Common.Exceptions;

namespace BrewBoxApi.Presentation.Filters;

public class NotFoundExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is NotFoundException notFoundException)
        {
            context.Result = new NotFoundObjectResult(new
            {
                error = "Not Found",
                message = notFoundException.Message,
                entity = notFoundException.EntityName,
                id = notFoundException.Id,
                expression = notFoundException.Expression
            });

            context.ExceptionHandled = true;
        }
    }
}