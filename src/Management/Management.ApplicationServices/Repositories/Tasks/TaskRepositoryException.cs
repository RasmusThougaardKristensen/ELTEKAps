namespace ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
public class TaskRepositoryException : Exception
{
    public TaskRepositoryException(Exception exception, string? message) : base(message) { }
}
