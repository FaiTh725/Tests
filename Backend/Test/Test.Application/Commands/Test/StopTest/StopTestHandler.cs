using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.TestSession;
using Test.Domain.Entities;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.Test.StopTest
{
    public class StopTestHandler :
        IRequestHandler<StopTestCommand, long>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly ITempDbService<TempTestSession> tempDbService;
        private readonly ITestEvaluatorService testEvaluatorService;
        private readonly IBackgroundJobService backgroundJobService;

        public StopTestHandler(
            INoSQLUnitOfWork unitOfWork,
            ITempDbService<TempTestSession> tempDbService,
            ITestEvaluatorService testEvaluatorService,
            IBackgroundJobService backgroundJobService)
        {
            this.unitOfWork = unitOfWork;
            this.tempDbService = tempDbService;
            this.testEvaluatorService = testEvaluatorService;
            this.backgroundJobService = backgroundJobService;
        }

        public async Task<long> Handle(
            StopTestCommand request, 
            CancellationToken cancellationToken)
        {
            var session = await tempDbService
                .GetEntity(request.SessionId);

            if(session is null)
            {
                throw new BadRequestException("Session doesnt exist");
            }

            var testSession = TestSession.Initialize(
                session.TestId, session.ProfileId);

            if(testSession.IsFailure)
            {
                throw new BadRequestException("Invalid request values - " +
                    $"{testSession.Error}");
            }

            var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
            
            try
            {
                var dbTestSession = await unitOfWork.SessionRepository
                    .AddTestSession(testSession.Value, transaction, cancellationToken);

                var lastAnswers = session.Answers
                    .GroupBy(x => x.QuestionId)
                    .ToDictionary(
                        x => x.Key,
                        x => x.OrderByDescending(x => x.SendTime).First());

                var testResult = await testEvaluatorService.Evaluate(
                    lastAnswers,
                    session.TestId,
                    dbTestSession.Id,
                    cancellationToken);
                
                if(testResult.IsFailure)
                {
                    throw new InternalServerErrorException(testResult.Error);
                }

                await unitOfWork.ProfileAnswerRepository
                    .AddProfileAnswers(
                        testResult.Value.ProfileAnswers,
                        transaction,
                        cancellationToken);

                var stopSessionResult = dbTestSession
                    .CloseSession(testResult.Value.Percent);

                if (stopSessionResult.IsFailure)
                {
                    throw new InternalServerErrorException("Error with calculate result test");
                }

                await unitOfWork.SessionRepository
                    .UpdateTestSession(dbTestSession.Id, dbTestSession, transaction, cancellationToken);

                await unitOfWork.CommitTransactionAsync(transaction, cancellationToken);

                if(session.TestDuration is not null)
                {
                    backgroundJobService.CancelJob(session.JobId!);
                }

                await tempDbService.RemoveEntity(session.Id);

                return dbTestSession.Id;
            }
            catch
            {
                await unitOfWork.RollBackTransactionAsync(transaction, cancellationToken);
                throw;
            }
        }
    }
}
