﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;
using Test.Domain.Enums;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Dal.Persistences
{
    public class MongoTest : IMongoPersistence<TestEntity, MongoTest>
    {
        [BsonId]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedTime { get; set; }

        public bool IsPublic { get; set; }

        public TestType TestType { get; set; }

        public long ProfileId { get; private set; }

        public List<long> QuestionsId { get; set; } = new List<long>();

        public TestEntity ConvertToDomainEntity()
        {
            var testEntity = TestEntity.Initialize(
                Name, Description, ProfileId, 
                TestType, IsPublic, QuestionsId);

            if(testEntity.IsFailure)
            {
                throw new InvalidDataException("Convert db entity to mongo entity");
            }

            // set id
            var type = typeof(TestEntity);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod!.Invoke(testEntity.Value, [Id]);

            return testEntity.Value;
        }

        public MongoTest ConvertToMongoEntity(TestEntity test)
        {
            Id = test.Id;
            Name = test.Name;
            Description = test.Description;
            CreatedTime = test.CreatedTime;
            IsPublic = test.IsPublic;
            TestType = test.TestType;
            ProfileId = test.ProfileId;
            QuestionsId = [.. test.QuestionsId];

            return this;
        }
    }
}
