using MediatR;
using Test.Application.Common.Interfaces;
using Test.Domain.Intrefaces;

namespace Test.Application.Commands.OneAnswerQuestionEntity.CreateQuestion
{
    public class CreateQuestionQuestionHandler :
        IRequestHandler<CreateQuestionCommand, long>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IBlobService blobService;

        public CreateQuestionQuestionHandler(
            INoSQLUnitOfWork unitOfWork,
            IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
        }

        public Task<long> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
