using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Commands.Test.StopTest;
using Test.Application.Common.Interfaces;
using Test.Application.Common.Wrappers;
using Test.Application.Contracts.TestSession;
using Test.Domain.Enums;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.Test.StartTest
{
    public class StartTestHandler :
        IRequestHandler<StartTestCommand, Guid>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly ITempDbService<TempTestSession> tempDbService;
        private readonly IBackgroundJobService backgroundJobService;

        public StartTestHandler(
            INoSQLUnitOfWork unitOfWork,
            ITempDbService<TempTestSession> tempDbService,
            IBackgroundJobService backgroundJobService)
        {
            this.unitOfWork = unitOfWork;
            this.tempDbService = tempDbService;
            this.backgroundJobService = backgroundJobService;
        }

        public async Task<Guid> Handle(
            StartTestCommand request, 
            CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.TestId, cancellationToken);

            if(test is null)
            {
                throw new BadRequestException("Test doesnt exist");
            }

            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.ProfileId, cancellationToken);

            if(profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var testSessionId = Guid.NewGuid();
            string? stopTestJobId = null;

            if(test.TestType == TestType.Timed)
            {
                stopTestJobId = backgroundJobService
                    .CreateDelayedJob<MediatorWrapper>(x => 
                    x.SendCommand(new StopTestCommand
                    {
                        SessionId = testSessionId
                    }), 
                    TimeSpan.FromMinutes(test.DurationInMinutes!.Value));
            }

            var tempSession = new TempTestSession
            {
                Id = testSessionId,
                ProfileId = profile.Id,
                StartTime = DateTime.UtcNow,
                TestDuration = test.TestType == TestType.Timed ? test.DurationInMinutes : null,
                TestId = test.Id,
                JobId = stopTestJobId
            };

            await tempDbService.AddEntity(tempSession);

            return testSessionId;
        }
    }
}
