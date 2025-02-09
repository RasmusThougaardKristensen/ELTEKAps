using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Update;
internal class UpdateTaskOperationHelper
{
    internal static TaskModel MapOperation(Guid id, Operation operation)
    {
        return new TaskModel(id,
            GetNewUserId(operation),
            GetNewCustomerId(operation),
            GetNewStatus(operation),
            GetNewDescription(operation),
            GetNewLocation(operation),
            GetNewTitle(operation));
    }

    private static string? GetNewTaskInformation(Operation operation, string operationDataConstant)
    {
        if (operation.Data is null || operation.Data.TryGetValue(operationDataConstant, out var newTaskInformation) is false)
            return null;

        return newTaskInformation;
    }

    private static Guid GetNewUserId(Operation operation)
    {
        return Guid.Parse(GetNewTaskInformation(operation, OperationDataConstants.NewTaskUserId));
    }

    private static Guid GetNewCustomerId(Operation operation)
    {
        return Guid.Parse(GetNewTaskInformation(operation, OperationDataConstants.NewTaskCustomerId));
    }

    private static string GetNewTitle(Operation operation)
    {
        return GetNewTaskInformation(operation, OperationDataConstants.NewTaskTitle);
    }

    private static string GetNewDescription(Operation operation)
    {
        return GetNewTaskInformation(operation, OperationDataConstants.NewTaskDescription);
    }

    private static Status GetNewStatus(Operation operation)
    {

        if (!Enum.TryParse(GetNewTaskInformation(operation, OperationDataConstants.NewTaskStatus), out Status taskStatusEntry))
        { }

        return taskStatusEntry;
    }

    private static string GetNewLocation(Operation operation)
    {
        return GetNewTaskInformation(operation, OperationDataConstants.NewTaskLocation);
    }
}
