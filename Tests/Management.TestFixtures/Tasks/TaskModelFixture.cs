using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Photos;
using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.TestFixtures.Tasks;

public static class TaskModelFixture
{
    public static TaskModelBuilder Builder() => new();

    public sealed class TaskModelBuilder
    {
        private Guid _id = Guid.NewGuid();
        private DateTime _createdUtc = DateTime.UtcNow.AddDays(-2);
        private DateTime _modifiedUtc = DateTime.UtcNow;
        private bool _deleted = false;
        private IEnumerable<CommentModel> _comments = new List<CommentModel>();
        private IEnumerable<PhotoModel> _photos = new List<PhotoModel>();
        private Guid _userId = Guid.NewGuid();
        private Guid _customerId = Guid.NewGuid();
        private Status _status = Status.New;  // Example default
        private string _description = "Default task description";
        private string _location = "Default location";
        private string _title = "Default title";

        internal TaskModelBuilder() { }

        public TaskModel Build()
        {
            return new TaskModel(
                _id,
                _createdUtc,
                _modifiedUtc,
                _deleted,
                _comments,
                _photos,
                _userId,
                _customerId,
                _status,
                _description,
                _location,
                _title
            );
        }

        public TaskModelBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public TaskModelBuilder WithCreatedUtc(DateTime createdUtc)
        {
            _createdUtc = createdUtc;
            return this;
        }

        public TaskModelBuilder WithModifiedUtc(DateTime modifiedUtc)
        {
            _modifiedUtc = modifiedUtc;
            return this;
        }

        public TaskModelBuilder WithDeleted(bool deleted = true)
        {
            _deleted = deleted;
            return this;
        }

        public TaskModelBuilder WithComments(IEnumerable<CommentModel> comments)
        {
            _comments = comments;
            return this;
        }

        public TaskModelBuilder WithPhotos(IEnumerable<PhotoModel> photos)
        {
            _photos = photos;
            return this;
        }

        public TaskModelBuilder WithUserId(Guid userId)
        {
            _userId = userId;
            return this;
        }

        public TaskModelBuilder WithCustomerId(Guid customerId)
        {
            _customerId = customerId;
            return this;
        }

        public TaskModelBuilder WithStatus(Status status)
        {
            _status = status;
            return this;
        }

        public TaskModelBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public TaskModelBuilder WithLocation(string location)
        {
            _location = location;
            return this;
        }

        public TaskModelBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }
    }
}
