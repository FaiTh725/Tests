using MediatR;

namespace Test.Application.Commands.Test.DeleteTest
{
    public class DeleteTestCommand : IRequest
    {
        public long Id { get; set; }
    }
}
