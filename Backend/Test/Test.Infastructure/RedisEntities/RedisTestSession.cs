using Redis.OM.Modeling;
using Test.Application.Contracts.ProfileAnswerEntity;

namespace Test.Infastructure.RedisEntities
{
    [Document(StorageType = StorageType.Json, Prefixes = ["Sessions"])]
    public class RedisTestSession
    {
        [RedisIdField]
        [Indexed]
        public Guid Id { get; set; }

        [Indexed]
        public long TestId { get; set; }

        [Indexed]
        public long ProfileId { get; set; }

        public DateTime StartTime { get; set; }

        public double? TestDuration { get; set; }

        [Indexed(CascadeDepth = 1)]
        public List<SessionProfileAnswer> Answers { get; set; } = new List<SessionProfileAnswer>();

        [Indexed]
        public string? JobId { get; set; } = string.Empty;
    }
}
