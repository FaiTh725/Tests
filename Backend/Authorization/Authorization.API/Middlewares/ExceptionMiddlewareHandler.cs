using Application.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Middlewares
{
    public class ExceptionMiddlewareHandler : IExceptionHandler
    {
        private readonly ILogger<ExceptionMiddlewareHandler> logger;
        private readonly IProblemDetailsService problemDetailsService;
        private readonly IHostApplicationLifetime host;

        public ExceptionMiddlewareHandler(
            ILogger<ExceptionMiddlewareHandler> logger,
            IProblemDetailsService problemDetailsService,
            IHostApplicationLifetime host)
        {
            this.logger = logger;
            this.problemDetailsService = problemDetailsService;
            this.host = host;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            if(exception is AppConfigurationException appException)
            {
                logger.LogError("Error with configuration, " +
                    "confifuration section with error - " + 
                    appException.SectionWithError);
            }

            httpContext.Response.StatusCode = exception switch
            { 
                BadRequestException => StatusCodes.Status400BadRequest,
                UnauthorizeException => StatusCodes.Status401Unauthorized,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext 
            { 
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Type = exception.GetType().Name,
                    Title = "Error Occured",
                    Detail = exception.Message,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                }
            });
        }
    }
}
