using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.Interfaces;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using QuestionEntity = Test.Domain.Entities.Question;

namespace Test.Application.Commands.Question.CreateQuestion
{
    public class CreateQuestionHandler :
        IRequestHandler<CreateQuestionCommand, long>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IBlobService blobService;

        public CreateQuestionHandler(
            INoSQLUnitOfWork unitOfWork,
            IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
        }

        public async Task<long> Handle(
            CreateQuestionCommand request, 
            CancellationToken cancellationToken)
        {
            var existedTest = await unitOfWork.TestRepository
                .GetTest(request.TestId, cancellationToken);
        
            if(existedTest is null)
            {
                throw new BadRequestException("Test doesnt exist");
            }

            var rightQuestions = request.Answers
                .Where(x => x.IsCorrect);

            if (!(request.QuestionType == QuestionType.OneAnswer &&
                rightQuestions.Count() == 1 ||
                request.QuestionType == QuestionType.ManyAnswers &&
                rightQuestions.Any()))
            {
                throw new BadRequestException("Answer should have 1 correct answer if the type is OneAnswer " +
                    "or at least one answer");
            }

            var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var question = QuestionEntity.Initialize(
                    request.TestQuestion, request.QuestionWeight, 
                    request.QuestionType, request.TestId);

                if(question.IsFailure)
                {
                    throw new BadRequestException("Invalid request - " +
                        $"{question.Value}");
                }

                var questionEntity = await unitOfWork.QuestionRepository
                    .AddQuestion(question.Value, transaction, cancellationToken);
                   
                var uploadFilesTasks = new List<Task<IEnumerable<string>>>();
                foreach (var questionAnswer in request.Answers)
                {
                    var questionAnswerEntity = QuestionAnswer.Initialize(
                        questionAnswer.Answer, questionAnswer.IsCorrect, questionEntity.Id);

                    if(questionAnswerEntity.IsFailure)
                    {
                        throw new BadRequestException("Error question answer");
                    }

                    var newQuestionAnswer = await unitOfWork.QuestionAnswerRepository
                        .AddQuestionAnswer(questionAnswerEntity.Value, transaction, cancellationToken);

                    if(questionAnswer.AnswerImages.Count != 0)
                    {
                        uploadFilesTasks.Add(blobService.UploadBlobs(
                            newQuestionAnswer.ImageFolder, questionAnswer.AnswerImages, 
                            cancellationToken));
                    }
                }

                await Task.WhenAll(uploadFilesTasks);
                await blobService.UploadBlobs(questionEntity.ImageFolder, request.QuestionImages, cancellationToken);

                await unitOfWork.CommitTransactionAsync(transaction, cancellationToken);

                return questionEntity.Id;
            }
            catch(ApiException)
            {
                await unitOfWork.RollBackTransactionAsync(transaction, cancellationToken);
                throw;
            }
            catch
            {
                await unitOfWork.RollBackTransactionAsync(transaction, cancellationToken);
                throw new InternalServerErrorException("Inner api exception");
            }
        }
    }
}
