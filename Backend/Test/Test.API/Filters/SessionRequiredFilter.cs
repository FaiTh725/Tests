using Application.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.TestSession;
using Test.Domain.Entities;

namespace Test.API.Filters
{
    public class SessionRequiredFilter : IAsyncActionFilter
    {
        private readonly ITempDbService<TempTestSession> tempDbService;

        public SessionRequiredFilter(
            ITempDbService<TempTestSession> tempDbService)
        {
            this.tempDbService = tempDbService;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, 
            ActionExecutionDelegate next)
        {
            var sessionId = context.HttpContext.Request.Cookies["test_session"];

            if(sessionId is null)
            {
                throw new BadRequestException("Session is missing");
            }

            if(!Guid.TryParse(sessionId, out Guid sessionGuid))
            {
                throw new BadRequestException("Session id has an invalid signature");
            }

            var session = await tempDbService.GetEntity(sessionGuid);

            if(session is null)
            {
                throw new BadRequestException($"Session doesnt exist");
            }

            context.HttpContext.Items.Add("SessionId", sessionId);

            await next();
        }
    }
}
