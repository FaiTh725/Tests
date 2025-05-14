using MediatR;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.Test.GetTestToPass
{
    public class GetTestToPassQuery : IRequest<TestToPassResponse>
    {
        public long Id { get; set; }
    }
}
