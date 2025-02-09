using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.ApplicationServices.Tasks.Create;
using ELTEKAps.Management.Domain.Customers;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;
using ELTEKAps.Management.Domain.Users;
using ELTEKAps.Management.TestFixtures.Customers;
using ELTEKAps.Management.TestFixtures.Operations;
using ELTEKAps.Management.TestFixtures.Tasks;
using ELTEKAps.Management.TestFixtures.Users;
using Management.Messages.External.Tasks.Create;
using Management.Messages.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Rebus.Bus;

namespace ELTEKAps.Management.Tests.Tasks;
internal class CreateTaskServiceTests
{
    private Mock<ITaskRepository> _taskRepositoryMock;
    private Mock<IOperationService> _operationServiceMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<IBus> _busMock;
    private Mock<ILogger<CreateTaskService>> _loggerMock;

    private CreateTaskService _createTaskService;

    [SetUp]
    public void Setup()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _operationServiceMock = new Mock<IOperationService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _busMock = new Mock<IBus>();
        _loggerMock = new Mock<ILogger<CreateTaskService>>();

        _createTaskService = new CreateTaskService(
            _taskRepositoryMock.Object,
            _operationServiceMock.Object,
            _userRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _loggerMock.Object,
            _busMock.Object
        );
    }

    #region RequestCreateTask

    [Test]
    public async Task RequestCreateTask_ReturnAcceptedOperationResult_OnHappyPath()
    {
        var userModel = UserModelFixture.Builder().Build();
        var customerModel = CustomerModelFixture.Builder().Build();
        var operation = OperationFixture.Builder().WithOperationName(OperationName.CreateCustomer).Build();
        var requestCreateTaskModel = TaskModelFixture.Builder()
            .WithUserId(userModel.Id)
            .WithCustomerId(customerModel.Id)
            .Build();


        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(userModel);
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(customerModel);
        _operationServiceMock.Setup(x => x.QueueOperation(It.IsAny<Operation>())).ReturnsAsync(operation);

        var requestCreateTaskResult = await _createTaskService.RequestCreateTask(requestCreateTaskModel, new OperationDetails("created-by"));

        Assert.That(requestCreateTaskResult.Status, Is.EqualTo(OperationResultStatus.Accepted));

        _operationServiceMock.Verify(x => x.QueueOperation(
            It.Is<Operation>(x => x.Name == OperationName.CreateTask)
            ));
        _userRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(userId => userId == requestCreateTaskModel.UserId)));
        _customerRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(customerId => customerId == customerModel.Id)));
        _busMock.Verify(Setup => Setup.Send(It.Is<RequestCreateTaskCommand>(msg => msg.RequestId == operation.RequestId), null));
    }

    [Test]
    public async Task RequestCreateTask_ReturnInvalidStateOperationResult_WhenUserDoesNotExist()
    {
        var userModel = UserModelFixture.Builder().Build();
        var requestCreateTaskModel = TaskModelFixture.Builder()
            .WithUserId(userModel.Id)
            .Build();


        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(null as UserModel);

        var requestCreateTaskResult = await _createTaskService.RequestCreateTask(requestCreateTaskModel, new OperationDetails("created-by"));

        Assert.That(requestCreateTaskResult.Status, Is.EqualTo(OperationResultStatus.InvalidState));

        _userRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(userId => userId == requestCreateTaskModel.UserId)));
        _busMock.Verify(Setup => Setup.Send(It.IsAny<RequestCreateTaskCommand>(), null), Times.Never);
    }

    [Test]
    public async Task RequestCreateTask_ReturnInvalidStateOperationResult_WhenCustomerDoesNotExist()
    {
        var userModel = UserModelFixture.Builder().Build();
        var customerModel = CustomerModelFixture.Builder().Build();
        var requestCreateTaskModel = TaskModelFixture.Builder()
            .WithUserId(userModel.Id)
            .WithCustomerId(customerModel.Id)
            .Build();


        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(userModel);
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(null as CustomerModel);

        var requestCreateTaskResult = await _createTaskService.RequestCreateTask(requestCreateTaskModel, new OperationDetails("created-by"));

        Assert.That(requestCreateTaskResult.Status, Is.EqualTo(OperationResultStatus.InvalidState));

        _userRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(userId => userId == requestCreateTaskModel.UserId)));
        _customerRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(customerId => customerId == customerModel.Id)));
        _busMock.Verify(Setup => Setup.Send(It.IsAny<RequestCreateTaskCommand>(), null), Times.Never);
    }
    #endregion

    [Test]
    public async Task CreateTask_CreatesTaskAndPublishSucceedEvent_OnHappyPath()
    {
        var createTaskModel = TaskModelFixture.Builder().Build();

        var operation = OperationFixture.Builder()
            .WithAddData(OperationDataConstants.CreateTaskUserId, createTaskModel.UserId.ToString())
            .WithAddData(OperationDataConstants.CreateTaskCustomerId, createTaskModel.CustomerId.ToString())
            .WithAddData(OperationDataConstants.CreateTaskStatus, createTaskModel.Status.ToString())
            .WithAddData(OperationDataConstants.CreateTaskDescription, createTaskModel.Description)
            .WithAddData(OperationDataConstants.CreateTaskLocation, createTaskModel.Location)
            .WithAddData(OperationDataConstants.CreateTaskTitle, createTaskModel.Title)
            .WithOperationName(OperationName.CreateTask).Build();

        _operationServiceMock.Setup(x => x.GetOperationByRequestId(It.IsAny<string>())).ReturnsAsync(operation);
        _taskRepositoryMock.Setup(x => x.Upsert(It.IsAny<TaskModel>())).ReturnsAsync(createTaskModel);

        await _createTaskService.CreateTask(operation.RequestId);

        _operationServiceMock.Verify(x => x.GetOperationByRequestId(It.Is<string>(requestId => requestId == operation.RequestId)));
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(
            It.Is<string>(requestId => requestId == operation.RequestId),
            It.Is<OperationStatus>(status => status == OperationStatus.Processing)));
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(
            It.Is<string>(requestId => requestId == operation.RequestId),
            It.Is<OperationStatus>(status => status == OperationStatus.Completed)));

        _busMock.Verify(bus => bus.Publish(It.Is<CreateTaskSucceedEvent>(msg => msg.TaskId == createTaskModel.Id), null));
    }

    [Test]
    public async Task CreateTask_Returns_WhenOperationDoesNotExist()
    {
        var invalidRequestId = Guid.NewGuid().ToString();

        var createTaskModel = TaskModelFixture.Builder().Build();

        _operationServiceMock.Setup(x => x.GetOperationByRequestId(It.IsAny<string>())).ReturnsAsync(null as Operation);

        await _createTaskService.CreateTask(invalidRequestId);

        _operationServiceMock.Verify(x => x.GetOperationByRequestId(It.Is<string>(requestId => requestId == invalidRequestId)));

        _busMock.Verify(bus => bus.Publish(It.Is<CreateTaskSucceedEvent>(msg => msg.TaskId == createTaskModel.Id), null), Times.Never);
    }

    [Test]
    public async Task CreateTask_PublishFailedEvent_WhenExceptionIsCaught()
    {
        var createTaskModel = TaskModelFixture.Builder().Build();

        var operation = OperationFixture.Builder()
            .WithAddData(OperationDataConstants.CreateTaskUserId, createTaskModel.UserId.ToString())
            .WithAddData(OperationDataConstants.CreateTaskCustomerId, createTaskModel.CustomerId.ToString())
            .WithAddData(OperationDataConstants.CreateTaskStatus, createTaskModel.Status.ToString())
            .WithAddData(OperationDataConstants.CreateTaskDescription, createTaskModel.Description)
            .WithAddData(OperationDataConstants.CreateTaskLocation, createTaskModel.Location)
            .WithAddData(OperationDataConstants.CreateTaskTitle, createTaskModel.Title)
            .WithOperationName(OperationName.CreateTask).Build();

        _operationServiceMock.Setup(x => x.GetOperationByRequestId(It.IsAny<string>())).ReturnsAsync(operation);
        _taskRepositoryMock.Setup(x => x.Upsert(It.IsAny<TaskModel>())).ThrowsAsync(new Exception());

        Assert.ThrowsAsync<Exception>(() => _createTaskService.CreateTask(operation.RequestId));

        _operationServiceMock.Verify(x => x.GetOperationByRequestId(It.Is<string>(requestId => requestId == operation.RequestId)));
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(
            It.Is<string>(requestId => requestId == operation.RequestId),
            It.Is<OperationStatus>(status => status == OperationStatus.Processing)));
        _operationServiceMock.Verify(x => x.UpdateOperationStatus(
            It.Is<string>(requestId => requestId == operation.RequestId),
            It.Is<OperationStatus>(status => status == OperationStatus.Failed)));

        _busMock.Verify(bus => bus.Publish(It.Is<CreateTaskFailedEvent>(msg => msg.RequestId == operation.RequestId), null));
    }
}
