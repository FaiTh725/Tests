using MediatR;
using Test.Domain.Enums;

namespace Test.Application.Commands.Test.CreateTest
{
    public class CreateTestCommand : IRequest<long>
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsPublic { get; set; }

        public TestType TestType { get; set; }

        public long ProfileId { get; set; }

        public double? DurationInMinutes { get; set; }
    }
}
