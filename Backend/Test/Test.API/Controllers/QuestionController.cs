using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.Question;
using Test.Application.Commands.Question.CreateQuestion;
using Test.Application.Contracts.File;
using Test.Application.Contracts.QuestionAnswerEntity;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController: ControllerBase
    {
        private readonly IMediator mediator;

        public QuestionController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> AddQuestion(
            [FromForm]CreateQuestionRequest request, CancellationToken cancellationToken)
        {
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
                }).ToList()
            };
            var questionId = await mediator.Send(createQuestionCommand, cancellationToken);

            return Ok(questionId);
        }
    }
}
