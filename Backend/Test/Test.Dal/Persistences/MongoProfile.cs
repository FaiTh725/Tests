using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoProfile : IMongoPersistence<Profile, MongoProfile>
    {
        [BsonId]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public Profile ConvertToDomainEntity()
        {
            var profileEntity = Profile.Initialize(
                Name, Email);

            if(profileEntity.IsFailure)
            {
                throw new InvalidDataException("Convert db entity to domain entity");
            }

            // set id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod!.Invoke(profileEntity.Value, [Id]);

            return profileEntity.Value;
        }

        public MongoProfile ConvertToMongoEntity(Profile profile)
        {
            Id = profile.Id;
            Name = profile.Name;
            Email = profile.Email;

            return this;
        }
    }
}
