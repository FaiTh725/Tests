using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileAnswerEntity;
using Test.Application.Contracts.TestSession;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.Test.SendTestAnswer
{
    public class SendTestAnswerHandler :
        IRequestHandler<SendTestAnswerCommand>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly ITempDbService<TempTestSession> tempDbService;

        public SendTestAnswerHandler(
            INoSQLUnitOfWork unitOfWork,
            ITempDbService<TempTestSession> tempDbService)
        {
            this.unitOfWork = unitOfWork;
            this.tempDbService = tempDbService;
        }

        public async Task Handle(
            SendTestAnswerCommand request, 
            CancellationToken cancellationToken)
        {
            var testSession = await tempDbService
                .GetEntity(request.SessionId);

            if(testSession is null)
            {
                throw new BadRequestException("Session doesnt exist");
            }

            var questionAnswers = await unitOfWork.QuestionAnswerRepository
                .GetQuestionAnswersByCriteria(new AnswersByQuestionIdSpecification(request.QuestionId), 
                cancellationToken);

            var isValidAnswers = request.QuestionAnswersId
                .ToHashSet()
                .IsSubsetOf(questionAnswers
                        .Select(x => x.Id));
        
            if(!isValidAnswers)
            {
                throw new BadRequestException("Invalid answers id, some answers dont exist");
            }

            testSession.Answers.Add(new SessionProfileAnswer
            {
                QuestionAnswersId = request.QuestionAnswersId,
                QuestionId = request.QuestionId,
                SendTime = DateTime.UtcNow,
            });

            await tempDbService.UpdateEntity(
                testSession.Id, testSession);
        }
    }
}
