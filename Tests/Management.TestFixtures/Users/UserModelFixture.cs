using ELTEKAps.Management.Domain.Users;

namespace ELTEKAps.Management.TestFixtures.Users;

public static class UserModelFixture
{
    public static UserModelBuilder Builder() => new();

    public sealed class UserModelBuilder
    {
        private Guid _id = Guid.NewGuid();
        private DateTime _createdUtc = DateTime.UtcNow.AddDays(-1);
        private DateTime _modifiedUtc = DateTime.UtcNow;
        private string _firebaseId = Guid.NewGuid().ToString();
        private string _name = "Default User Name";
        private string _email = "default.user@example.com";
        private bool _deleted = false;

        internal UserModelBuilder() { }

        public UserModel Build()
        {
            return new UserModel(
                _id,
                _createdUtc,
                _modifiedUtc,
                _deleted,
                _firebaseId,
                _name,
                _email
            );
        }

        public UserModelBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public UserModelBuilder WithCreatedUtc(DateTime createdUtc)
        {
            _createdUtc = createdUtc;
            return this;
        }

        public UserModelBuilder WithModifiedUtc(DateTime modifiedUtc)
        {
            _modifiedUtc = modifiedUtc;
            return this;
        }

        public UserModelBuilder WithFirebaseId(string firebaseId)
        {
            _firebaseId = firebaseId;
            return this;
        }

        public UserModelBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public UserModelBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public UserModelBuilder WithDeleted(bool deleted)
        {
            _deleted = deleted;
            return this;
        }
    }
}
