using MassTransit;
using MediatR;
using Test.Application.Contracts.Question;
using Test.Domain.Events;

namespace Test.Application.EventHandler.QuestionEventHandler
{
    public class QuestionDeletedEventHandler :
        INotificationHandler<QuestionDeletedEvent>
    {
        private readonly IPublishEndpoint bus;

        public QuestionDeletedEventHandler(
            IPublishEndpoint bus)
        {
            this.bus = bus;
        }

        public async Task Handle(
            QuestionDeletedEvent notification, 
            CancellationToken cancellationToken)
        {
            await bus.Publish(new DeleteDependentsQuestionEntities
            {
                QuestionId = notification.QuestionId,
            }, cancellationToken);
        }
    }
}
