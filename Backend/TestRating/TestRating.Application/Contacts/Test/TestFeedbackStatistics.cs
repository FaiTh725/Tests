namespace TestRating.Application.Contacts.Test
{
    public class TestFeedbackStatistics
    {
        public long TestId { get; set; }

        public double AverageRating { get; set; }

        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();
    }
}
