using ELTEKAps.Management.Domain.Photos;

namespace ELTEKAps.Management.TestFixtures.Photos;

public static class PhotoModelFixture
{
    public static PhotoModelBuilder Builder() => new();

    public sealed class PhotoModelBuilder
    {
        private Guid _id = Guid.NewGuid();
        private DateTime _createdUtc = DateTime.UtcNow.AddDays(-1);
        private DateTime _modifiedUtc = DateTime.UtcNow;
        private bool _deleted = false;
        private string _photoData = "default-photo-data";
        private Guid _taskId = Guid.NewGuid();

        internal PhotoModelBuilder() { }

        public PhotoModel Build()
        {
            return new PhotoModel(
                _id,
                _createdUtc,
                _modifiedUtc,
                _deleted,
                _photoData,
                _taskId
            );
        }

        public PhotoModelBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public PhotoModelBuilder WithCreatedUtc(DateTime createdUtc)
        {
            _createdUtc = createdUtc;
            return this;
        }

        public PhotoModelBuilder WithModifiedUtc(DateTime modifiedUtc)
        {
            _modifiedUtc = modifiedUtc;
            return this;
        }

        public PhotoModelBuilder WithDeleted(bool deleted = true)
        {
            _deleted = deleted;
            return this;
        }

        public PhotoModelBuilder WithPhotoData(string photoData)
        {
            _photoData = photoData;
            return this;
        }

        public PhotoModelBuilder WithTaskId(Guid taskId)
        {
            _taskId = taskId;
            return this;
        }
    }
}
