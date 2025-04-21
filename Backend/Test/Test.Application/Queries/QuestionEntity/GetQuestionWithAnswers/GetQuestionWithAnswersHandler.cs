using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.Question;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Domain.Intrefaces;

namespace Test.Application.Queries.QuestionEntity.GetQuestionWithAnswers
{
    public class GetQuestionWithAnswersHandler :
        IRequestHandler<GetQuestionWithAnswersQuery, QuestionWithAnswersResponse>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IBlobService blobService;

        public GetQuestionWithAnswersHandler(
            INoSQLUnitOfWork unitOfWork,
            IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
        }

        public async Task<QuestionWithAnswersResponse> Handle(
            GetQuestionWithAnswersQuery request, 
            CancellationToken cancellationToken)
        {
            var question = await unitOfWork.QuestionRepository
                .GetQuestion(request.Id, cancellationToken);

            if(question is null)
            {
                throw new NotFoundException("Question doesnt exist");
            }

            var questionAnswers = await unitOfWork.QuestionAnswerRepository
                .GetQuestionAnswersByCriteria(
                new AnswersByQuestionsIdSpecification([question.Id]), 
                cancellationToken);

            var answersResponseTasks = questionAnswers.Select(async x => new QuestionAnswerResponse
            {
                Id = x.Id,
                Answer = x.Answer,
                IsCorrect = x.IsCorrect,
                QuestionId = x.QuestionId,
                QuestionAnswersImageUrls = (await blobService.GetBlobFolder(x.ImageFolder, cancellationToken)).ToList()
            }).ToList();

            var answersResponse = await Task.WhenAll(answersResponseTasks);

            return new QuestionWithAnswersResponse
            {
                Id = question.Id,
                QuestionType = question.QuestionType,
                QuestionWeight = question.QuestionWeight,
                TestQuestion = question.TestQuestion,
                Answers = answersResponse.ToList(),
                QuestionImages = (await blobService.GetBlobFolder(question.ImageFolder, cancellationToken)).ToList()
            };
        }
    }
}
