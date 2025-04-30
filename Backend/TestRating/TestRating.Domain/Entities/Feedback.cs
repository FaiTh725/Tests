using CSharpFunctionalExtensions;

namespace TestRating.Domain.Entities
{
    public class Feedback : Entity
    {
        public string ImageFolder { get => $"FeedBack-{Id}"; }

        public string Text { get; private set; }

        public Profile Owner { get; private set; }

        public DateTime SendTime { get; private set; }

        public DateTime UpdateTime { get; private set; }

        public Feedback() { }
    }
}
