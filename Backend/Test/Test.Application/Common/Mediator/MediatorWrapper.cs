using MediatR;

namespace Test.Application.Common.Mediator
{
    public class MediatorWrapper
    {
        private readonly IMediator mediator;

        public MediatorWrapper(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task SendCommand<TCommand>(TCommand request)
            where TCommand : IBaseRequest
        {
            await mediator.Send(request, CancellationToken.None);
        }
    }
}
