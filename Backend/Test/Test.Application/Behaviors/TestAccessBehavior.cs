using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using Test.Application.Queries.TestAccessEntity.Specifications;
using Test.Domain.Intrefaces;

namespace Test.Application.Behaviors
{
    public class TestAccessBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : ITestVisibleAccess
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public TestAccessBehavior(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.TestId, cancellationToken);
        
            if(test is null)
            {
                throw new BadRequestException("Test doesnt exist");
            }

            if(test.IsPublic ||
                test.ProfileId == request.ProfileId)
            {
                return await next(cancellationToken);
            }

            var profileGroups = await unitOfWork.ProfileGroupRepository
                .GetProfileGroupsByCriteria(new GroupsByProfileIdSpecification(request.ProfileId), cancellationToken);

            var avaliableAccessId = profileGroups
                .Select(x => x.Id)
                .ToList();
            avaliableAccessId.Add(request.ProfileId);

            var testAccesses = await unitOfWork.AccessRepository
                .GetAccessesByCriteria(
                new AccessesByTargetEntitySpecification(avaliableAccessId), 
                cancellationToken);
            
            if(!testAccesses.Any())
            {
                throw new ForbiddenAccessException("Current test is private");
            }

            return await next(cancellationToken);
        }
    }
}
