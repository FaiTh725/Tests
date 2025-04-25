using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;
using Test.Domain.Intrefaces;

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
                throw new NotFoundException("Question doesnt exist");
            }

            var test = await unitOfWork.TestRepository
                .GetTest(question.TestId, cancellationToken);

            if(test is null)
            {
                throw new InternalServerErrorException("Invalid data in database");
            }

            var profile = await unitOfWork.ProfileRepository
                .GetProfile(test.ProfileId, cancellationToken);

            if (request.Role != "Admin" &&
                (profile is null || profile.Email != request.Email))
            {
                throw new ForbiddenAccessException("Only the owner or an admin has access to the test");
            }

            throw new ForbiddenAccessException("Only a owner and admin have access to the test");

        }
    }
}
