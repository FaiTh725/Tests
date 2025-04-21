using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoProfileGroup : IMongoPersistence<ProfileGroup, MongoProfileGroup>
    {
        [BsonId]
        public long Id { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public long OwnerId { get; set; }

        public List<long> MembersId { get; set; } = new List<long>();

        public ProfileGroup ConvertToDomainEntity()
        {
            var profileGroupEntity = ProfileGroup.Initialize(
                GroupName,
                OwnerId,
                MembersId);

            if(profileGroupEntity.IsFailure)
            {
                throw new InvalidDataException("Error convert db entity to domain entity");
            }

            // set Id
            var type = typeof(ProfileGroup);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(profileGroupEntity.Value, [Id]);

            return profileGroupEntity.Value;
        }

        public MongoProfileGroup ConvertToMongoEntity(ProfileGroup group)
        {
            Id = group.Id;
            GroupName = group.GroupName;
            MembersId = [.. group.MembersId];

            return this;
        }
    }
}
