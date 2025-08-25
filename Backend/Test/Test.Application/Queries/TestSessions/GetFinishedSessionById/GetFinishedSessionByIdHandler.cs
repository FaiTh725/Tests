using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileAnswerEntity;
using Test.Application.Contracts.Question;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Application.Contracts.TestSession;
using Test.Application.Queries.ProfileAnswers.Specifications;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Application.Queries.QuestionEntity.Specifications;
using Test.Domain.Entities;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.TestSessions.GetFinishedSessionById
{
    public class GetFinishedSessionByIdHandler : 
        IRequestHandler<GetFinishedSessionByIdQuery, SessionInfo>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IBlobService blobService;

        public GetFinishedSessionByIdHandler(
            INoSQLUnitOfWork unitOfWork,
            IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
        }

        public async Task<SessionInfo> Handle(
            GetFinishedSessionByIdQuery request, 
            CancellationToken cancellationToken)
        {
            var session = await unitOfWork.SessionRepository
                .GetFinishedTest(request.Id, cancellationToken);
        
            if(session is null)
            {
                throw new NotFoundException("Session doesnt exist");
            }

            var test = await unitOfWork.TestRepository
                .GetTest(session.TestId, cancellationToken);

            if(test is null)
            {
                throw new InternalServerErrorException("Session with a nonexistent test");
            }

            var testQuestions = await unitOfWork.QuestionRepository
                .GetQuestionsByCriteria(
                new QuestionsByTestIdSpecification(test.Id), 
                cancellationToken);

            var questionIdList = testQuestions.Select(x => x.Id).ToList();

            var allAnswersForQuestions = await unitOfWork.QuestionAnswerRepository
                .GetQuestionAnswersByCriteria(
                new AnswersByQuestionsIdSpecification(questionIdList), 
                cancellationToken);

            // questionId - questionAnswers
            var questionsDictinary = allAnswersForQuestions
                .GroupBy(x => x.QuestionId)
                .ToDictionary(x => x.Key, x => x.ToList());

            var profileAnswersForAllQuestions = await unitOfWork.ProfileAnswerRepository
                    .GetProfileAnswersByCriteria(
                new GetProfileQuestionAnswerForSessionSpecification(session.Id), 
                cancellationToken);

            // questionId - profileQuestionAnswers 
            var profileQuestionAnswers = profileAnswersForAllQuestions
                    .GroupBy(x => x.QuestionId)
                    .ToDictionary(x => x.Key, x => x.ToList());

            // questionId - question images
            var questionImages = testQuestions
                .ToDictionary(
                    x => x.Id, 
                    x => blobService
                        .GetBlobFolder(x.ImageFolder, cancellationToken));

            // answerId - answer images
            var answerImages = allAnswersForQuestions
                .ToDictionary(
                    x => x.Id, 
                    x => blobService
                        .GetBlobFolder(x.ImageFolder, cancellationToken));

            await Task.WhenAll(questionImages.Values);
            await Task.WhenAll(answerImages.Values);

            var profileAnswers = testQuestions.Select(x => {

                var questionAnswers = questionsDictinary.GetValueOrDefault(x.Id, new List<QuestionAnswer>());
                var profileAnswers = profileQuestionAnswers.GetValueOrDefault(x.Id, new List<ProfileAnswer>());

                return new QuestionProfileAnswers
                {
                    QuestionId = x.Id,
                    QuestionImages = questionImages[x.Id].Result.ToList(),
                    QuestionText = x.TestQuestion,
                    Answers = questionAnswers.Select(y => new QuestionAnswerToPassTest
                    {
                        Id = y.Id,
                        Answer = y.Answer,
                        QuestionAnswerImages = answerImages[y.Id].Result.ToList()
                    }).ToList(),
                    ProfileAnswers = profileAnswers.Select(y => new ProfileQuestionAnswers
                    {
                        IsCorrectAnswer = y.IsCorrect,
                        QuestionId = x.Id,
                        ProfileAnswersId = y.QuestionAnswersId
                    }).ToList()

                };
            });

            return new SessionInfo 
            { 
                Id = session.Id,
                TestName = test.Name,
                TestId = session.TestId,
                ProfileId = session.ProfileId,
                IsEnded = session.IsEnded,
                Percent = session.Percent,
                EndTime = session.EndTime!.Value,
                StartTime = session.StartTime,
                QuestionsDetailResult = profileAnswers.ToList()
            };
        }
    }
}
