using Application.Shared.Exceptions;
using MassTransit;
using MediatR;
using Test.Application.Common.Interfaces;
using Test.Domain.Entities;
using Test.Domain.Intrefaces;
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

            
            await unitOfWork.BeginTransactionAsync(cancellationToken);

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
                    .AddQuestion(question.Value, cancellationToken);

                // спотыкаемся здесь, так как hangfire не может сериализовать stream
                // короче меняем на masstransit
                //backgroundJobService.CreateFireAndForgetJob<IBlobService>(x => 
                //    x.UploadBlobs(question.Value.ImageFolder, request.QuestionImages, cancellationToken));
                // тут тоже спотыкаемся тк он тоже не сможет мне сериализовать stream
                //await bus.Publish(new UploadFilesMessage
                //{
                //    Folder = question.Value.ImageFolder,
                //    Files = request.QuestionImages
                //}, cancellationToken);

                // загрузка blob ов
                // таски на загрузку файлов                    
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
                        .AddQuestionAnswer(questionAnswerEntity.Value, cancellationToken);

                    if(questionAnswer.AnswerImages.Count != 0)
                    {
                        uploadFilesTasks.Add(blobService.UploadBlobs(
                            newQuestionAnswer.ImageFolder, questionAnswer.AnswerImages, 
                            cancellationToken));
                    }
                }

                await Task.WhenAll(uploadFilesTasks);
                await blobService.UploadBlobs(questionEntity.ImageFolder, request.QuestionImages, cancellationToken);

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return questionEntity.Id;
            }
            catch(ApiException)
            {
                await unitOfWork.RollBackTransactionAsync(cancellationToken);
                throw;
            }
            catch
            {
                await unitOfWork.RollBackTransactionAsync(cancellationToken);
                throw new InternalServerErrorException("Inner api exception");
            }
        }
    }
}
