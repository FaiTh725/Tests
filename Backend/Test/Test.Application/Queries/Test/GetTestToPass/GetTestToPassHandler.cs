using Application.Shared.Exceptions;
using MediatR;
using System.Collections.Concurrent;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.Question;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Application.Queries.QuestionEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.Test.GetTestToPass
{
    public class GetTestToPassHandler :
        IRequestHandler<GetTestToPassQuery, TestToPassResponse>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IBlobService blobService;

        public GetTestToPassHandler(
            INoSQLUnitOfWork unitOfWork,
            IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
        }

        public async Task<TestToPassResponse> Handle(
            GetTestToPassQuery request, 
            CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.Id, cancellationToken);

            if (test is null) 
            {
                throw new NotFoundException("Test doesnt exist");
            }

            var testQuestions = await unitOfWork.QuestionRepository
                .GetQuestionsByCriteria(
                new QuestionsByTestIdSpecification(test.Id), 
                cancellationToken);

            var questionsToPass = new ConcurrentBag<QuestionToPassTest>();

            await Parallel.ForEachAsync(testQuestions, async (question, token) =>
            {
                var images = await blobService.GetBlobFolder(question.ImageFolder, token);
                var answers = await GetQuestionsAnswer(question.Id, token);

                var questionToPass = new QuestionToPassTest
                {
                    Id = question.Id,
                    QuestionType = question.QuestionType.ToString(),
                    TestQuestion = question.TestQuestion,
                    QuestionImages = images.ToList(),
                    Answers = answers.ToList()
                };

                questionsToPass.Add(questionToPass);
            });

            return new TestToPassResponse
            {
                Id = test.Id,
                Description = test.Description,
                Name = test.Name,
                TestType = test.TestType.ToString(),
                Questions = questionsToPass.ToList()
            };
        }

        private async Task<IEnumerable<QuestionAnswerToPassTest>> GetQuestionsAnswer(
                long questionId,
                CancellationToken cancellationToken = default)
        {
            var answers = await unitOfWork.QuestionAnswerRepository
                .GetQuestionAnswersByCriteria(new AnswersByQuestionIdSpecification(questionId), cancellationToken);

            var answersToPass = new ConcurrentBag<QuestionAnswerToPassTest>();

            await Parallel.ForEachAsync(answers, async (answer, token) =>
            {
                var images = await blobService.GetBlobFolder(answer.ImageFolder, token);

                var answerToPass = new QuestionAnswerToPassTest
                {
                    Id = answer.Id,
                    Answer = answer.Answer,
                    QuestionAnswerImages = images.ToList()
                };

                answersToPass.Add(answerToPass);
            });

            return answersToPass;
        }
    }
}
