using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.Question;
using Test.Application.Commands.Question.CreateQuestion;
using Test.Application.Commands.Question.DeleteQuestion;
using Test.Application.Commands.Question.UpdateQuestion;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.File;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Application.Queries.QuestionEntity.GetQuestionWithAnswers;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController: ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IProfileService profileService;

        public QuestionController(
            IMediator mediator,
            IProfileService profileService)
        {
            this.mediator = mediator;
            this.profileService = profileService;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> AddQuestion(
            [FromForm]CreateQuestionRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            var createQuestionCommand = new CreateQuestionCommand
            {
                TestId = request.TestId,
                QuestionType = request.QuestionType,
                TestQuestion = request.TestQuestion,
                QuestionWeight = request.QuestionWeight,
                QuestionImages = request.QuestionImages?.Select(x => new FileModel
                {
                    Stream = x.OpenReadStream(),
                    ContentType = x.ContentType,
                    Name = x.Name,
                }).ToList() ?? new List<FileModel>(),
                Answers = request.Answers.Select(x => new CreateQuestionAnswer
                {
                    Answer = x.Answer,
                    IsCorrect = x.IsCorrect,
                    AnswerImages = x.AnswerImages?.Select(y => new FileModel 
                    { 
                        Stream = y.OpenReadStream(),
                        ContentType = y.ContentType,
                        Name = y.Name
                    }).ToList() ?? new List<FileModel>()
                }).ToList(),
                OwnerId = profile.Id,
                Role = profile.Role
            };

            var questionId = await mediator.Send(createQuestionCommand, cancellationToken);

            var question = await mediator.Send(new GetQuestionWithAnswersQuery
            {
                Id = questionId,
            }, cancellationToken);

            return Ok(question);
        }

        [HttpDelete("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteQuestion(
            long questionId, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            await mediator.Send(new DeleteQuestionCommand 
            { 
                QuestionId = questionId,
                OwnerId = profile.Id,
                Role = profile.Role
            }, 
            cancellationToken);

            return Ok();
        }

        [HttpPatch("[action]")]
        [Authorize]
        public async Task<IActionResult> UpdateQuestion(
            UpdateQuestionRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            var questionId = await mediator.Send(new UpdateQuestionCommand
            {
                QuestionId = request.Id,
                QuestionWeight = request.QuestionWeight,
                TestQuestion = request.TestQuestion,
                Role = profile.Role,
                OwnerId = profile.Id
            }, 
            cancellationToken);

            var question = await mediator.Send(new GetQuestionWithAnswersQuery
            {
                Id = questionId,
            }, cancellationToken);

            return Ok(question);
        }
    }
}
