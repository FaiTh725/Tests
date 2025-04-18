using MediatR;
using Test.Domain.Events;
using Test.Domain.Intrefaces;

namespace Test.Application.EventHandler.TestEventHandler
{
    public class TestDeletedEventHandler : INotificationHandler<TestDeletedEvent>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public TestDeletedEventHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        // TODO
        public async Task Handle(
            TestDeletedEvent notification, 
            CancellationToken cancellationToken)
        {
            // delete all dependent question and their answers
            //var dependentQuestions = await unitOfWork.QuestionRepository
            throw new NotImplementedException();
        }
    }
}
