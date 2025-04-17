﻿using MediatR;
using Test.Application.Contracts.File;
using Test.Application.Contracts.QuestionAnswerEntity;

namespace Test.Application.Commands.OneAnswerQuestionEntity.CreateQuestion
{
    public class CreateQuestionCommand : 
        IRequest<long>
    {
        public List<FileModel> QuestionImages { get; set; } = new List<FileModel>();

        public string TestQuestion { get; set; } = string.Empty;

        public int QuestionWeight { get; set; }

        public long TestId { get; set; }

        public required CreateQuestionAnswer Answer { get; set; }
    }
}
