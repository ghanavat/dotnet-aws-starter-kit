using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.DotnetAws.Api.Middleware;

[ExcludeFromCodeCoverage]
public sealed class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    
    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server responded with error",
            Instance = httpContext.Request.Path,
            Detail = $"There has been a problem with your request. {exception.Message}",
            Type = exception.GetType().Name
        }, cancellationToken: cancellationToken);
        
        _logger.LogError("There has been a problem with your request.");

        return true;
    }
}
