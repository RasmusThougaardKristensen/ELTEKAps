using ELTEKAps.Management.Domain.Comments;

namespace ELTEKAps.Management.TestFixtures.Comments;

public static class CommentModelFixture
{
    public static CommentModelBuilder Builder() => new();

    public sealed class CommentModelBuilder
    {
        private Guid _id = Guid.NewGuid();
        private DateTime _createdUtc = DateTime.UtcNow.AddDays(-1);
        private DateTime _modifiedUtc = DateTime.UtcNow;
        private bool _deleted = false;
        private string _comment = "Default comment text";
        private Guid _taskId = Guid.NewGuid();

        internal CommentModelBuilder() { }

        public CommentModel Build()
        {
            return new CommentModel(
                _id,
                _createdUtc,
                _modifiedUtc,
                _deleted,
                _comment,
                _taskId
            );
        }

        public CommentModelBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public CommentModelBuilder WithCreatedUtc(DateTime createdUtc)
        {
            _createdUtc = createdUtc;
            return this;
        }

        public CommentModelBuilder WithModifiedUtc(DateTime modifiedUtc)
        {
            _modifiedUtc = modifiedUtc;
            return this;
        }

        public CommentModelBuilder WithDeleted(bool deleted = true)
        {
            _deleted = deleted;
            return this;
        }

        public CommentModelBuilder WithCommentText(string commentText)
        {
            _comment = commentText;
            return this;
        }

        public CommentModelBuilder WithTaskId(Guid taskId)
        {
            _taskId = taskId;
            return this;
        }
    }
}
