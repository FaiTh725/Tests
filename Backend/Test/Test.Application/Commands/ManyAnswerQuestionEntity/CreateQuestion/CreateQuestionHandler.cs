using MediatR;

namespace Test.Application.Commands.ManyAnswerQuestionEntity.CreateQuestion
{
    public class CreateQuestionHandler :
        IRequestHandler<CreateQuestionCommand, long>
    {
        public Task<long> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
