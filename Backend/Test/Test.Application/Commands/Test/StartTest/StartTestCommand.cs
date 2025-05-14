using MediatR;
using Test.Application.Common.BehaviorsInterfaces;

namespace Test.Application.Commands.Test.StartTest
{
    public class StartTestCommand: 
        IRequest<Guid>,
        ITestVisibleAccess
    {
        public long TestId { get; set; }

        public long ProfileId { get; set; }
    }
}
