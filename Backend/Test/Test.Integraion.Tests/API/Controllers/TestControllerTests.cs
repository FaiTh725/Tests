using Test.Application.Commands.Test.CreateTest;

namespace Test.Integration.Tests.API.Controllers
{
    public class TestControllerTests : BaseIntegrationTest
    {
        public TestControllerTests(CustomWebFactory factory) 
            : base(factory)
        {}

        [Fact]
        public async Task Test()
        {
            var request = new CreateTestCommand
            {
                Description = "Test",
                IsPublic = true,
                Name = "Test",
                ProfileId = 1,
                TestType = Domain.Enums.TestType.Timed,
                DurationInMinutes = 14
            };

            var response = await sender.Send(request, CancellationToken.None);
        }
    }
}
