using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.TestFixtures.Operations;

public static class OperationFixture
{
    public static OperationBuilder Builder() => new();

    public sealed class OperationBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _requestId = "default-request-id";
        private string _createdBy = "test-user";
        private Guid _taskId = Guid.NewGuid();
        private OperationName _operationName = OperationName.CreateTask;
        private OperationStatus _operationStatus = OperationStatus.Queued;
        private DateTime _createdUtc = DateTime.UtcNow.AddDays(-2);
        private DateTime _modifiedUtc = DateTime.UtcNow;
        private DateTime? _completedUtc = null;
        private Dictionary<string, string>? _data;

        internal OperationBuilder() { }

        public Operation Build()
        {
            return new Operation(
                _id,
                _requestId,
                _createdBy,
                _taskId,
                _operationName,
                _operationStatus,
                _createdUtc,
                _modifiedUtc,
                _completedUtc,
                _data
            );
        }

        public OperationBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public OperationBuilder WithRequestId(string requestId)
        {
            _requestId = requestId;
            return this;
        }

        public OperationBuilder WithCreatedBy(string createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public OperationBuilder WithTaskId(Guid taskId)
        {
            _taskId = taskId;
            return this;
        }

        public OperationBuilder WithOperationName(OperationName name)
        {
            _operationName = name;
            return this;
        }

        public OperationBuilder WithStatus(OperationStatus status)
        {
            _operationStatus = status;
            return this;
        }

        public OperationBuilder WithCreatedUtc(DateTime createdUtc)
        {
            _createdUtc = createdUtc;
            return this;
        }

        public OperationBuilder WithModifiedUtc(DateTime modifiedUtc)
        {
            _modifiedUtc = modifiedUtc;
            return this;
        }

        public OperationBuilder WithCompletedUtc(DateTime? completedUtc)
        {
            _completedUtc = completedUtc;
            return this;
        }

        public OperationBuilder WithClearData()
        {
            _data = null;
            return this;
        }

        public OperationBuilder WithAddData(string key, string value)
        {
            _data ??= new Dictionary<string, string>();
            _data.Add(key, value);
            return this;
        }
    }
}
