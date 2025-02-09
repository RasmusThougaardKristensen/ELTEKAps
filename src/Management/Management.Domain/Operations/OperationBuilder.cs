using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Customers;
using ELTEKAps.Management.Domain.Photos;
using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.Domain.Operations;
public static class OperationBuilder
{
    /// <summary>
    /// Generic operation factory
    /// </summary>
    private static Operation CreateOperation(
        Guid taskId,
        OperationName operationName,
        string createdBy,
        Dictionary<string, string>? data
    )
    {
        return new Operation(
            id: Guid.NewGuid(),
            requestId: Guid.NewGuid().ToString(),
            createdBy: createdBy,
            taskId: taskId,
            name: operationName,
            status: OperationStatus.Queued,
            createdUtc: DateTime.UtcNow,
            modifiedUtc: DateTime.UtcNow,
            completedUtc: null,
            data: data
        );
    }

    #region Task Operations

    public static Operation CreateTask(TaskModel taskModel, string createdBy)
    {
        var data = new Dictionary<string, string>()
        {
            { OperationDataConstants.CreateTaskUserId, taskModel.UserId.ToString() },
            { OperationDataConstants.CreateTaskCustomerId, taskModel.CustomerId.ToString() },
            { OperationDataConstants.CreateTaskTitle, taskModel.Title },
            { OperationDataConstants.CreateTaskDescription, taskModel.Description },
            { OperationDataConstants.CreateTaskStatus, taskModel.Status.ToString() },
            { OperationDataConstants.CreateTaskLocation, taskModel.Location }
        };

        return CreateOperation(
            taskId: taskModel.Id,
            operationName: OperationName.CreateTask,
            createdBy: createdBy,
            data: data
        );
    }

    public static Operation UpdateTask(TaskModel currentTaskModel, Guid newUserId, Guid newCustomerId, string newTitle,
        string newDescription, Status newStatus, string newLocation, string createdBy)
    {
        var data = new Dictionary<string, string>
        {
            { OperationDataConstants.CurrentTaskUserId, currentTaskModel.UserId.ToString() },
            { OperationDataConstants.CurrentTaskCustomerId, currentTaskModel.CustomerId.ToString() },
            { OperationDataConstants.CurrentTaskTitle, currentTaskModel.Title },
            { OperationDataConstants.CurrentTaskDescription, currentTaskModel.Description },
            { OperationDataConstants.CurrentTaskStatus, currentTaskModel.Status.ToString() },
            { OperationDataConstants.CurrentTaskLocation, currentTaskModel.Location },

            { OperationDataConstants.NewTaskUserId, newUserId.ToString() },
            { OperationDataConstants.NewTaskCustomerId, newCustomerId.ToString() },
            { OperationDataConstants.NewTaskTitle, newTitle },
            { OperationDataConstants.NewTaskDescription, newDescription },
            { OperationDataConstants.NewTaskStatus, newStatus.ToString() },
            { OperationDataConstants.NewTaskLocation, newLocation }
        };

        return CreateOperation(
            taskId: currentTaskModel.Id,
            operationName: OperationName.UpdateTask,
            createdBy: createdBy,
            data: data
        );
    }

    public static Operation SoftDeleteTask(Guid taskId, string createdBy)
    {
        return CreateOperation(
            taskId: taskId,
            operationName: OperationName.SoftDeleteTask,
            createdBy: createdBy,
            data: null
        );
    }

    #endregion

    #region Comment Operations

    public static Operation CreateComment(CommentModel commentModel, string createdBy)
    {
        var data = new Dictionary<string, string>
        {
            { "Comment", commentModel.Comment },
            { "TaskId", commentModel.TaskId.ToString() }
        };

        return CreateOperation(
            taskId: commentModel.Id,
            operationName: OperationName.CreateComment,
            createdBy: createdBy,
            data: data
        );
    }

    public static Operation UpdateComment(CommentModel commentModel, string createdBy)
    {
        var data = new Dictionary<string, string>
        {
            { "Comment", commentModel.Comment },
            { "TaskId", commentModel.TaskId.ToString() }
        };

        return CreateOperation(
            taskId: commentModel.Id,
            operationName: OperationName.UpdateComment,
            createdBy: createdBy,
            data: data
        );
    }

    public static Operation SoftDeleteComment(Guid commentId, string createdBy)
    {
        return CreateOperation(
            taskId: commentId,
            operationName: OperationName.SoftDeleteComment,
            createdBy: createdBy,
            data: null
        );
    }

    #endregion

    #region Photo Operations

    public static Operation CreatePhoto(PhotoModel photoModel, string createdBy)
    {
        var data = new Dictionary<string, string>
        {
            { "PhotoId", photoModel.Id.ToString() },
            { "PhotoData", photoModel.PhotoData },
            { "TaskId", photoModel.TaskId.ToString() }
        };

        return CreateOperation(
            taskId: photoModel.Id,
            operationName: OperationName.CreatePhoto,
            createdBy: createdBy,
            data: data
        );
    }

    public static Operation SoftDeletePhoto(Guid photoId, string createdBy)
    {
        return CreateOperation(
            taskId: photoId,
            operationName: OperationName.SoftDeletePhoto,
            createdBy: createdBy,
            data: null
        );
    }

    #endregion

    #region Customer Operations

    /// <summary>
    /// Queues creation of a Customer.
    /// </summary>
    public static Operation CreateCustomer(CustomerModel customerModel, string createdBy)
    {
        var data = new Dictionary<string, string>
        {
            { "CustomerName", customerModel.CustomerName },
            { "PhoneNumber", customerModel.PhoneNumber },
            { "Email", customerModel.Email }
        };

        return CreateOperation(
            taskId: customerModel.Id,
            operationName: OperationName.CreateCustomer,
            createdBy: createdBy,
            data: data
        );
    }

    /// <summary>
    /// Queues update of a Customer.
    /// </summary>
    public static Operation UpdateCustomer(CustomerModel customerModel, string createdBy)
    {
        var data = new Dictionary<string, string>
        {
            { "CustomerName", customerModel.CustomerName },
            { "PhoneNumber", customerModel.PhoneNumber },
            { "Email", customerModel.Email }
        };

        return CreateOperation(
            taskId: customerModel.Id,
            operationName: OperationName.UpdateCustomer,
            createdBy: createdBy,
            data: data
        );
    }

    /// <summary>
    /// Queues soft-deletion of a Customer.
    /// </summary>
    public static Operation SoftDeleteCustomer(Guid customerId, string createdBy)
    {
        return CreateOperation(
            taskId: customerId,
            operationName: OperationName.SoftDeleteCustomer,
            createdBy: createdBy,
            data: null
        );
    }

    #endregion
}
