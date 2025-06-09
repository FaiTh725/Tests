using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.Question;
using Test.Application.Queries.QuestionEntity.GetQuestionWithAnswers;
using Test.Application.Queries.QuestionEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.QuestionEntity.GetQuestionsByTestId
{
    public class GetQuestionsByTestIdHandler :
        IRequestHandler<GetQuestionsByTestIdQuery, IEnumerable<QuestionWithAnswersResponse>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IMediator mediator;

        public GetQuestionsByTestIdHandler(
            INoSQLUnitOfWork unitOfWork,
            IMediator mediator)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<QuestionWithAnswersResponse>> Handle(
            GetQuestionsByTestIdQuery request, 
            CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.TestId, cancellationToken);

            if(test is null)
            {
                throw new NotFoundException("Test doesnt exist");
            }

            var testQuestion = await unitOfWork.QuestionRepository
                .GetQuestionsByCriteria(
                    new QuestionsByTestIdSpecification(request.TestId), 
                cancellationToken);

            var tasksGetQuestion = testQuestion
                .Select(x => mediator
                    .Send(new GetQuestionWithAnswersQuery
                    {
                        Id = x.Id
                    }, 
                    cancellationToken));

            var testQuestionWithAnswers = await Task.WhenAll(tasksGetQuestion);

            return testQuestionWithAnswers;
        }
    }
}
