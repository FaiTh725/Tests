using MediatR;
using Microsoft.Extensions.Logging;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.TestSession;

namespace Test.Application.Commands.Test.ClearTestProgress
{
    public class ClearTestProgressHandler :
        IRequestHandler<ClearTestProgressCommand>
    {
        private readonly ILogger<ClearTestProgressHandler> logger;
        private readonly ITempDbService<TempTestSession> tempDbService;

        public ClearTestProgressHandler(
            ILogger<ClearTestProgressHandler> logger,
            ITempDbService<TempTestSession> tempDbService)
        {
            this.logger = logger;
            this.tempDbService = tempDbService;
        }

        public async Task Handle(
            ClearTestProgressCommand request, 
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Clean test session job starts");

            var sessions = await tempDbService.GetAllEntities();

            var inActiveSessions = sessions
                .Where(x => x.TestDuration == null &&
                            (x.StartTime < DateTime.UtcNow.AddMinutes(-20) ||
                            x.Answers.MaxBy(a => a.SendTime)?.SendTime < DateTime.UtcNow.AddMinutes(-20)))
                .Select(x => x.Id);

            var removeInactiveSessionsTasks = inActiveSessions
                .Select(tempDbService.RemoveEntity)
                .ToList();

            await Task.WhenAll(removeInactiveSessionsTasks);
        }
    }
}
