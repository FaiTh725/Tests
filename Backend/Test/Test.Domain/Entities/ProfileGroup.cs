using CSharpFunctionalExtensions;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public class ProfileGroup: Entity
    {
        public string GroupName { get; private set; }

        public long OwnerId { get; private set; }

        public List<long> MembersId { get; private set; }

        private ProfileGroup(
            string gropName, 
            long ownerId)
        {
            GroupName = gropName;

            MembersId = new List<long>();
            OwnerId = ownerId;
        }

        private ProfileGroup(
            string gropName,
            long ownerId,
            List<long> membersId) :
            this(gropName, ownerId)
        {
            MembersId = membersId;
        }

        public static Result<ProfileGroup> Initialize(
            string groupName,
            long ownerId)
        {
            var isValid = Validate(groupName);

            if(isValid.IsFailure)
            {
                return Result.Failure<ProfileGroup>(isValid.Error);
            }

            return Result.Success(new ProfileGroup(
                groupName,
                ownerId));
        }

        public static Result<ProfileGroup> Initialize(
            string groupName,
            long ownerId,
            List<long> membersId)
        {
            var isValid = Validate(groupName);

            if (isValid.IsFailure)
            {
                return Result.Failure<ProfileGroup>(isValid.Error);
            }

            if(membersId is null)
            {
                return Result.Failure<ProfileGroup>("MembersId list is null");
            }

            return Result.Success(new ProfileGroup(
                groupName,
                ownerId,
                membersId));
        }

        private static Result Validate(
            string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName) ||
                groupName.Length < ProfileGroupValidator.MIN_GROUP_NAME_LENGTH ||
                groupName.Length > ProfileGroupValidator.MAX_GROUP_NAME_LENGTH)
            {
                return Result.Failure("Name group is null or length greate " +
                    $"{ProfileGroupValidator.MAX_GROUP_NAME_LENGTH} or less {ProfileGroupValidator.MIN_GROUP_NAME_LENGTH}");
            }

            return Result.Success();
        }
    }
}
