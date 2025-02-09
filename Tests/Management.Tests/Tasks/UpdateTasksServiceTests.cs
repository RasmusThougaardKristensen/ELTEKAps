using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.ApplicationServices.Tasks.Update;
using ELTEKAps.Management.Domain.Customers;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;
using ELTEKAps.Management.Domain.Users;
using ELTEKAps.Management.TestFixtures.Customers;
using ELTEKAps.Management.TestFixtures.Operations;
using ELTEKAps.Management.TestFixtures.Tasks;
using ELTEKAps.Management.TestFixtures.Users;
using Management.Messages.External.Tasks.Update;
using Management.Messages.Tasks.Update;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Rebus.Bus;

namespace ELTEKAps.Management.Tests.Tasks;
internal class UpdateTasksServiceTests
{
    private Mock<ITaskRepository> _taskRepositoryMock;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IOperationService> _operationServiceMock;
    private Mock<ILogger<UpdateTaskService>> _loggerMock;
    private Mock<IBus> _busMock;

    private UpdateTaskService _updateTaskService;

    [SetUp]
    public void Setup()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _operationServiceMock = new Mock<IOperationService>();
        _loggerMock = new Mock<ILogger<UpdateTaskService>>();
        _busMock = new Mock<IBus>();

        _updateTaskService = new UpdateTaskService(_taskRepositoryMock.Object, _operationServiceMock.Object, _loggerMock.Object, _customerRepositoryMock.Object, _busMock.Object, _userRepositoryMock.Object);
    }

    #region RequestUpdateTask

    [Test]
    public async Task RequestUpdateTask_ReturnsAcceptedOperationResult()
    {
        var customerModel = CustomerModelFixture.Builder().Build();
        var userModel = UserModelFixture.Builder().Build();
        var newTaskModel = TaskModelFixture.Builder().WithCustomerId(customerModel.Id).WithUserId(userModel.Id).Build();
        var operation = OperationFixture.Builder().WithOperationName(OperationName.UpdateTask).Build();

        _taskRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(newTaskModel);
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(customerModel);
        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(userModel);
        _operationServiceMock.Setup(x => x.QueueOperation(It.IsAny<Operation>())).ReturnsAsync(operation);

        var requestUpdateTaskResult = await _updateTaskService.RequestUpdateTask(newTaskModel, new OperationDetails(operation.CreatedBy));

        Assert.That(requestUpdateTaskResult.Status, Is.EqualTo(OperationResultStatus.Accepted));

        _taskRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(taskId => taskId == newTaskModel.Id)));
        _customerRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(customerId => customerId == newTaskModel.CustomerId)));
        _userRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(userId => userId == newTaskModel.UserId)));
        _busMock.Verify(x => x.Send(It.Is<RequestUpdateTaskCommand>(msg => msg.RequestId == operation.RequestId && msg.TaskId == newTaskModel.Id), null));
    }

    [Test]
    public async Task RequestUpdateTask_ReturnsInvalidStateOperationResult_WhenTaskDoesNotExist()
    {
        var newTaskModel = TaskModelFixture.Builder().Build();

        _taskRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(null as TaskModel);

        var requestUpdateTaskResult = await _updateTaskService.RequestUpdateTask(newTaskModel, new OperationDetails("CreatedBy"));

        Assert.That(requestUpdateTaskResult.Status, Is.EqualTo(OperationResultStatus.InvalidState));

        _taskRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(taskId => taskId == newTaskModel.Id)));
        _customerRepositoryMock.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
        _busMock.Verify(x => x.Send(It.IsAny<RequestUpdateTaskCommand>(), null), Times.Never);
    }

    [Test]
    public async Task RequestUpdateTask_ReturnsInvalidStateOperationResult_WhenUserDoesNotExist()
    {
        var newTaskModel = TaskModelFixture.Builder().Build();

        _taskRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(newTaskModel);
        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(null as UserModel);

        var requestUpdateTaskResult = await _updateTaskService.RequestUpdateTask(newTaskModel, new OperationDetails("CreatedBy"));

        Assert.That(requestUpdateTaskResult.Status, Is.EqualTo(OperationResultStatus.InvalidState));

        _taskRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(taskId => taskId == newTaskModel.Id)));
        _userRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(userId => userId == newTaskModel.UserId)));
        _busMock.Verify(x => x.Send(It.IsAny<RequestUpdateTaskCommand>(), null), Times.Never);
    }

    [Test]
    public async Task RequestUpdateTask_ReturnsInvalidStateOperationResult_WhenUserIsDeleted()
    {
        var userModel = UserModelFixture.Builder().WithDeleted(true).Build();
        var newTaskModel = TaskModelFixture.Builder().WithUserId(userModel.Id).Build();

        _taskRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(newTaskModel);
        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(userModel);

        var requestUpdateTaskResult = await _updateTaskService.RequestUpdateTask(newTaskModel, new OperationDetails("CreatedBy"));

        Assert.That(requestUpdateTaskResult.Status, Is.EqualTo(OperationResultStatus.InvalidState));

        _taskRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(taskId => taskId == newTaskModel.Id)));
        _userRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(userId => userId == newTaskModel.UserId)));
        _busMock.Verify(x => x.Send(It.IsAny<RequestUpdateTaskCommand>(), null), Times.Never);
    }


    [Test]
    public async Task RequestUpdateTask_ReturnsInvalidStateOperationResult_WhenCustomerDoesNotExist()
    {
        var userModel = UserModelFixture.Builder().Build();
        var newTaskModel = TaskModelFixture.Builder().WithCustomerId(Guid.NewGuid()).WithUserId(userModel.Id).Build();

        _taskRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(newTaskModel);
        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(userModel);
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(null as CustomerModel);

        var requestUpdateTaskResult = await _updateTaskService.RequestUpdateTask(newTaskModel, new OperationDetails("CreatedBy"));

        Assert.That(requestUpdateTaskResult.Status, Is.EqualTo(OperationResultStatus.InvalidState));

        _taskRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(taskId => taskId == newTaskModel.Id)));
        _userRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(userId => userId == newTaskModel.UserId)));
        _customerRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(customerId => customerId == newTaskModel.CustomerId)));
        _busMock.Verify(x => x.Send(It.IsAny<RequestUpdateTaskCommand>(), null), Times.Never);
    }

    [Test]
    public async Task RequestUpdateTask_ReturnsInvalidStateOperationResult_WhenCustomerIsDeleted()
    {
        var userModel = UserModelFixture.Builder().Build();
        var customerModel = CustomerModelFixture.Builder().WithDeleted(true).Build();
        var newTaskModel = TaskModelFixture.Builder().WithCustomerId(customerModel.Id).WithUserId(userModel.Id).Build();

        _taskRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(newTaskModel);
        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(userModel);
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(customerModel);

        var requestUpdateTaskResult = await _updateTaskService.RequestUpdateTask(newTaskModel, new OperationDetails("CreatedBy"));

        Assert.That(requestUpdateTaskResult.Status, Is.EqualTo(OperationResultStatus.InvalidState));

        _taskRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(taskId => taskId == newTaskModel.Id)));
        _userRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(userId => userId == newTaskModel.UserId)));
        _customerRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(customerId => customerId == newTaskModel.CustomerId)));
        _busMock.Verify(x => x.Send(It.IsAny<RequestUpdateTaskCommand>(), null), Times.Never);
    }
    #endregion

    #region UpdateTask

    [Test]
    public async Task UpdateTask_UpdateTaskInformation_HappyPath()
    {
        var taskModel = TaskModelFixture.Builder().Build();

        var operation = OperationFixture.Builder()
            .WithAddData(OperationDataConstants.NewTaskUserId, Guid.NewGuid().ToString())
            .WithAddData(OperationDataConstants.NewTaskCustomerId, Guid.NewGuid().ToString())
            .WithAddData(OperationDataConstants.NewTaskStatus, Status.InProgress.ToString())
            .WithAddData(OperationDataConstants.NewTaskDescription, "New Description")
            .WithAddData(OperationDataConstants.NewTaskLocation, "New Location")
            .WithAddData(OperationDataConstants.NewTaskTitle, "New title")
            .Build();

        _operationServiceMock.Setup(x => x.GetOperationByRequestId(It.IsAny<string>())).ReturnsAsync(operation);
        _taskRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(taskModel);

        await _updateTaskService.UpdateTask(operation.RequestId, taskModel.Id);


        _operationServiceMock.Verify(x => x.GetOperationByRequestId(It.Is<string>(requestId => requestId == operation.RequestId)));
        _taskRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(taskId => taskId == taskModel.Id)));
        _taskRepositoryMock.Verify(x => x.UpdateTaskInformation(It.Is<TaskModel>(task => task.Id == taskModel.Id)));
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(It.Is<string>(requestId => requestId == operation.RequestId), It.Is<OperationStatus>(operationStatus => operationStatus == OperationStatus.Processing)));
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(It.Is<string>(requestId => requestId == operation.RequestId), It.Is<OperationStatus>(operationStatus => operationStatus == OperationStatus.Completed)));
        _busMock.Verify(x => x.Publish(It.Is<TaskUpdateSucceedEvent>(msg => msg.TaskId == taskModel.Id), null));

        _busMock.Verify(x => x.Publish(It.IsAny<TaskUpdateFailedEvent>(), null), Times.Never);
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(It.Is<string>(requestId => requestId == operation.RequestId), It.Is<OperationStatus>(operationStatus => operationStatus == OperationStatus.Failed)), Times.Never);
    }

    [Test]
    public async Task UpdateTask_Returns_WhenOperationIsNull()
    {
        _operationServiceMock.Setup(x => x.GetOperationByRequestId(It.IsAny<string>())).ReturnsAsync(null as Operation);

        await _updateTaskService.UpdateTask("requestId", Guid.NewGuid());

        _operationServiceMock.Verify(x => x.GetOperationByRequestId(It.Is<string>(requestId => requestId == "requestId")));
    }

    [Test]
    public void UpdateTask_PublishFailedEvent_WhenExpectationIsCaught()
    {
        var exceptionMessage = "An error occurred while updating task information";
        var taskModel = TaskModelFixture.Builder().Build();

        var operation = OperationFixture.Builder()
            .WithAddData(OperationDataConstants.NewTaskUserId, Guid.NewGuid().ToString())
            .WithAddData(OperationDataConstants.NewTaskCustomerId, Guid.NewGuid().ToString())
            .WithAddData(OperationDataConstants.NewTaskStatus, Status.InProgress.ToString())
            .WithAddData(OperationDataConstants.NewTaskDescription, "New Description")
            .WithAddData(OperationDataConstants.NewTaskLocation, "New Location")
            .WithAddData(OperationDataConstants.NewTaskTitle, "New title")
            .Build();

        _operationServiceMock.Setup(x => x.GetOperationByRequestId(It.IsAny<string>())).ReturnsAsync(operation);
        _taskRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(taskModel);
        _taskRepositoryMock.Setup(x => x.UpdateTaskInformation(It.IsAny<TaskModel>())).ThrowsAsync(new TaskRepositoryException(new Exception(), exceptionMessage));


        Assert.ThrowsAsync<TaskRepositoryException>(() => _updateTaskService.UpdateTask(operation.RequestId, taskModel.Id));

        _operationServiceMock.Verify(x => x.GetOperationByRequestId(It.Is<string>(requestId => requestId == operation.RequestId)));
        _taskRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(taskId => taskId == taskModel.Id)));
        _taskRepositoryMock.Verify(x => x.UpdateTaskInformation(It.Is<TaskModel>(task => task.Id == taskModel.Id)));
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(It.Is<string>(requestId => requestId == operation.RequestId), It.Is<OperationStatus>(operationStatus => operationStatus == OperationStatus.Processing)));
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(It.Is<string>(requestId => requestId == operation.RequestId), It.Is<OperationStatus>(operationStatus => operationStatus == OperationStatus.Failed)));
        _busMock.Verify(x => x.Publish(It.Is<TaskUpdateFailedEvent>(msg => msg.TaskId == taskModel.Id && msg.ErrorMessage == exceptionMessage), null));

        _busMock.Verify(x => x.Publish(It.IsAny<TaskUpdateSucceedEvent>(), null), Times.Never);
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(It.Is<string>(requestId => requestId == operation.RequestId), It.Is<OperationStatus>(operationStatus => operationStatus == OperationStatus.Completed)), Times.Never);
    }
    #endregion
}
