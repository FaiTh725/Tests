using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.Question.UpdateQuestion
{
    public class UpdateQuestionHandler :
        IRequestHandler<UpdateQuestionCommand, long>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public UpdateQuestionHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(
            UpdateQuestionCommand request, 
            CancellationToken cancellationToken)
        {
            var question = await unitOfWork.QuestionRepository
                .GetQuestion(request.QuestionId, cancellationToken);

            if(question is null)
            {
                throw new BadRequestException("Question doesnt exist");
            }

            var isValidUpdate = question.Update(
                request.TestQuestion,
                request.QuestionWeight);

            if(isValidUpdate.IsFailure)
            {
                throw new BadRequestException("Invalid values to update - " +
                    $"{isValidUpdate.Error}");
            }

            await unitOfWork.QuestionRepository
                .UpdateQuestion(question.Id, question, cancellationToken: cancellationToken);

            return question.Id;
        }
    }
}
