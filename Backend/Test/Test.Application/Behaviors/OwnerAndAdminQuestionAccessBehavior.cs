using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Domain.Interfaces;

namespace Test.Application.Behaviors
{
    public class OwnerAndAdminQuestionAccessBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IOwnerAndAdminQuestionAccess
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public OwnerAndAdminQuestionAccessBehavior(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var question = await unitOfWork.QuestionRepository
                .GetQuestion(request.QuestionId, cancellationToken);

            if(question is null)
            {
                throw new BadRequestException("Question doesnt exist");
            }

            var test = await unitOfWork.TestRepository
                .GetTest(question.TestId, cancellationToken);

            if(test is null)
            {
                throw new InternalServerErrorException("Invalid data in database");
            }

            if (request.Role != "Admin" &&
                request.OwnerId != test.ProfileId)
            {
                throw new ForbiddenAccessException("Only the owner or an admin have access to the test");
            }

            return await next(cancellationToken);

        }
    }
}
