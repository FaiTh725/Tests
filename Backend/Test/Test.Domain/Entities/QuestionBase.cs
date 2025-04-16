﻿using CSharpFunctionalExtensions;

namespace Test.Domain.Entities
{
    public abstract class QuestionBase : Entity
    {
        public string ImageFolder { get => $"Question-{Id}"; }

        public string TestQuestion {  get; private set; }

        public int QuestionWeight { get; private set; }

        public long TestId { get; private set; }

        protected QuestionBase(
            string testQuestion,
            int questionWeight,
            long testId)
        {
            TestQuestion = testQuestion;
            QuestionWeight = questionWeight;
            TestId = testId;
        }
    }
}
