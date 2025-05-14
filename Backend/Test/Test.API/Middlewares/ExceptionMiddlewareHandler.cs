using Application.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Test.API.Middlewares
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
            if (exception is AppConfigurationException appException)
            {
                logger.LogError("Error with configuration, " +
                    "configuration section with error - " +
                    appException.SectionWithError);

                host.StopApplication();
            }

            httpContext.Response.StatusCode = exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                UnauthorizeException => StatusCodes.Status401Unauthorized,
                ForbiddenAccessException => StatusCodes.Status403Forbidden,
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
                    Title = "Error Occurred",
                    Detail = exception.Message,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                }
            });
        }
    }
}
