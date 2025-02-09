using System.Runtime.Serialization;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Create;
[Serializable]
internal class CreateTaskServiceException : Exception
{
    public CreateTaskServiceException()
    {
    }

    public CreateTaskServiceException(string? message) : base(message)
    {
    }

    public CreateTaskServiceException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CreateTaskServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}