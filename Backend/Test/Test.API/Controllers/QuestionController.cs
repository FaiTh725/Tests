using Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.Question;
using Test.Application.Commands.Question.CreateQuestion;
using Test.Application.Commands.Question.DeleteQuestion;
using Test.Application.Commands.Question.UpdateQuestion;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.File;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.QuestionAnswerEntity;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController: ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ITokenService<ProfileToken> tokenService;

        public QuestionController(
            IMediator mediator,
            ITokenService<ProfileToken> tokenService)
        {
            this.mediator = mediator;
            this.tokenService = tokenService;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> AddQuestion(
            [FromForm]CreateQuestionRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

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
                Email = profileToken.Value.Email,
                Role = profileToken.Value.Role
            };
            var questionId = await mediator.Send(createQuestionCommand, cancellationToken);

            return Ok(questionId);
        }

        [HttpDelete("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteQuestion(
            long questionId, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            await mediator.Send(new DeleteQuestionCommand 
            { 
                QuestionId = questionId,
                Email = profileToken.Value.Email,
                Role = profileToken.Value.Role
            }, 
            cancellationToken);

            return Ok();
        }

        [HttpPatch("[action]")]
        [Authorize]
        public async Task<IActionResult> UpdateQuestion(
            UpdateQuestionRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var questionId = await mediator.Send(new UpdateQuestionCommand
            {
                QuestionId = request.Id,
                QuestionWeight = request.QuestionWeight,
                TestQuestion = request.TestQuestion,
                Role = profileToken.Value.Role,
                Email = profileToken.Value.Email
            }, 
            cancellationToken);

            return Ok(questionId);
        }
    }
}
