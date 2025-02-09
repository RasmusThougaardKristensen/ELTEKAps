using ELTEKAps.Management.ApplicationServices.Comments;
using ELTEKAps.Management.ApplicationServices.Comments.Create;
using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Comments;
using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.TestFixtures.Comments;
using ELTEKAps.Management.TestFixtures.Operations;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Comments
{
    [TestFixture]
    public class CreateCommentServiceTests
    {
        private Mock<ICommentRepository> _mockCommentRepository;
        private Mock<IOperationService> _mockOperationService;
        private Mock<ILogger<CreateCommentService>> _mockLogger;
        private CreateCommentService _createCommentService;

        [SetUp]
        public void SetUp()
        {
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockOperationService = new Mock<IOperationService>();
            _mockLogger = new Mock<ILogger<CreateCommentService>>();
            _createCommentService = new CreateCommentService(
                _mockCommentRepository.Object,
                _mockOperationService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task RequestCreateComment_ValidComment_ReturnsAcceptedOperationResult()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("Valid comment")
                .Build();
            var operationDetails = new OperationDetails("test-user");
            var operation = OperationFixture.Builder()
                .WithOperationName(OperationName.CreateComment)
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ReturnsAsync(operation);

            // Act
            var result = await _createCommentService.RequestCreateComment(commentModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Accepted));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Once);
            _mockCommentRepository.Verify(repo => repo.Upsert(commentModel), Times.Once);
        }

        [Test]
        public async Task RequestCreateComment_InvalidComment_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("") // Invalid comment
                .Build();
            var operationDetails = new OperationDetails("test-user");

            // Act
            var result = await _createCommentService.RequestCreateComment(commentModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCommentRepository.Verify(repo => repo.Upsert(It.IsAny<CommentModel>()), Times.Never);
        }

        [Test]
        public void RequestCreateComment_ExceptionThrown_ThrowsCommentOperationException()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("Valid comment")
                .Build();
            var operationDetails = new OperationDetails("test-user");

            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CommentOperationException>(() =>
                _createCommentService.RequestCreateComment(commentModel, operationDetails)
            );
        }

        [Test]
        public async Task CreateComment_ValidComment_CreatesCommentSuccessfully()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("Valid comment")
                .Build();

            // Act
            await _createCommentService.CreateComment(commentModel);

            // Assert
            _mockCommentRepository.Verify(repo => repo.Upsert(commentModel), Times.Once);
        }

        [Test]
        public void CreateComment_ExceptionThrown_ThrowsCommentCreationException()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("Valid comment")
                .Build();

            _mockCommentRepository
                .Setup(repo => repo.Upsert(It.IsAny<CommentModel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CommentCreationException>(() =>
                _createCommentService.CreateComment(commentModel)
            );
        }
    }
}
