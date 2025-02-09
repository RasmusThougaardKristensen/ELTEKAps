using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Create;
internal class CreateTaskOperationHelper
{
    internal static TaskModel MapOperation(Operation operation)
    {
        return TaskModel.Create(
            GetUserId(operation),
            GetNewCustomerId(operation),
            GetNewStatus(operation),
            GetNewDescription(operation),
            GetNewLocation(operation),
            GetNewTitle(operation));
    }

    private static string? GetCreateTaskInformation(Operation operation, string operationDataConstant)
    {
        if (operation.Data is null || operation.Data.TryGetValue(operationDataConstant, out var createTaskInformation) is false)
            return null;

        return createTaskInformation;
    }

    private static Guid GetUserId(Operation operation)
    {
        return Guid.Parse(GetCreateTaskInformation(operation, OperationDataConstants.CreateTaskUserId));
    }


    private static Guid GetNewCustomerId(Operation operation)
    {
        return Guid.Parse(GetCreateTaskInformation(operation, OperationDataConstants.CreateTaskCustomerId));
    }

    private static string GetNewTitle(Operation operation)
    {
        return GetCreateTaskInformation(operation, OperationDataConstants.CreateTaskTitle);
    }

    private static string GetNewDescription(Operation operation)
    {
        return GetCreateTaskInformation(operation, OperationDataConstants.CreateTaskDescription);
    }

    private static Status GetNewStatus(Operation operation)
    {

        if (!Enum.TryParse(GetCreateTaskInformation(operation, OperationDataConstants.CreateTaskStatus), out Status taskStatusEntry))
        { }

        return taskStatusEntry;
    }

    private static string GetNewLocation(Operation operation)
    {
        return GetCreateTaskInformation(operation, OperationDataConstants.CreateTaskLocation);
    }
}
