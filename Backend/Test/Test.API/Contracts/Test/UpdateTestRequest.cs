namespace Test.API.Contracts.Test
{
    public class UpdateTestRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsPublic { get; set; }

        public long TestId { get; set; }
    }
}
