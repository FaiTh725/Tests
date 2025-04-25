using MediatR;

namespace Test.Application.Commands.Test.SendTestAnswer
{
    public class SendTestAnswerCommand : IRequest
    {
        public Guid SessionId { get; set; }

        public long QuestionId { get; set; }

        public List<long> QuestionAnswersId { get; set; } = new List<long>();
    }
}
