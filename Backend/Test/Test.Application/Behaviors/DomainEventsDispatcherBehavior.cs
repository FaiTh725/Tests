using MediatR;
using Test.Domain.Intrefaces;

namespace Test.Application.Behaviors
{
    public class DomainEventsDispatcherBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IMediator mediator;

        public DomainEventsDispatcherBehavior(
            INoSQLUnitOfWork unitOfWork,
            IMediator mediator)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken);

            var trackedEntities = unitOfWork
                .GetTrackedEntities()
                .Where(x => x.DomainEvents.Any())
                .ToList();

            var events = trackedEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            foreach(var trackedEntity in trackedEntities)
            {
                trackedEntity.ClearDomainEvents();
            }

            foreach(var domainEvent in events)
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }

            return response;
        }
    }
}
