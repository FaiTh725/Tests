using MediatR;

namespace Test.Application.Commands.Test.StartTest
{
    public class StartTestCommand: IRequest<Guid>
    {
        public long TestId { get; set; }

        public long ProfileId { get; set; }
    }
}
