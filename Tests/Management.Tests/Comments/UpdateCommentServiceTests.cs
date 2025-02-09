using ELTEKAps.Management.ApplicationServices.Comments;
using ELTEKAps.Management.ApplicationServices.Comments.Update;
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
    public class UpdateCommentServiceTests
    {
        private Mock<ICommentRepository> _mockCommentRepository;
        private Mock<IOperationService> _mockOperationService;
        private Mock<ILogger<UpdateCommentService>> _mockLogger;
        private UpdateCommentService _updateCommentService;

        [SetUp]
        public void SetUp()
        {
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockOperationService = new Mock<IOperationService>();
            _mockLogger = new Mock<ILogger<UpdateCommentService>>();
            _updateCommentService = new UpdateCommentService(
                _mockCommentRepository.Object,
                _mockOperationService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task RequestUpdateComment_ValidComment_ReturnsAcceptedOperationResult()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("Valid comment")
                .Build();
            var operationDetails = new OperationDetails("test-user");
            var operation = OperationFixture.Builder()
                .WithOperationName(OperationName.UpdateComment)
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockCommentRepository
                .Setup(repo => repo.GetById(commentModel.Id))
                .ReturnsAsync(commentModel); // Simulate existing comment
            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ReturnsAsync(operation);

            // Act
            var result = await _updateCommentService.RequestUpdateComment(commentModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Accepted));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Once);
            _mockCommentRepository.Verify(repo => repo.Upsert(commentModel), Times.Once);
        }

        [Test]
        public async Task RequestUpdateComment_InvalidComment_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("") // Invalid comment (empty content)
                .Build();
            var operationDetails = new OperationDetails("test-user");

            // Act
            var result = await _updateCommentService.RequestUpdateComment(commentModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCommentRepository.Verify(repo => repo.Upsert(It.IsAny<CommentModel>()), Times.Never);
        }

        [Test]
        public async Task RequestUpdateComment_CommentDoesNotExist_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("Valid comment")
                .Build();
            var operationDetails = new OperationDetails("test-user");

            _mockCommentRepository
                .Setup(repo => repo.GetById(commentModel.Id))
                .ReturnsAsync((CommentModel)null); // Simulate non-existent comment

            // Act
            var result = await _updateCommentService.RequestUpdateComment(commentModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCommentRepository.Verify(repo => repo.Upsert(It.IsAny<CommentModel>()), Times.Never);
        }

        [Test]
        public void RequestUpdateComment_ExceptionThrown_ThrowsCommentOperationException()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("Valid comment")
                .Build();
            var operationDetails = new OperationDetails("test-user");

            _mockCommentRepository
                .Setup(repo => repo.GetById(commentModel.Id))
                .ReturnsAsync(commentModel); // Simulate existing comment
            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CommentOperationException>(() =>
                _updateCommentService.RequestUpdateComment(commentModel, operationDetails)
            );
        }

        [Test]
        public async Task UpdateComment_ValidComment_UpdatesCommentSuccessfully()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("Valid comment")
                .Build();

            // Act
            await _updateCommentService.UpdateComment(commentModel);

            // Assert
            _mockCommentRepository.Verify(repo => repo.Upsert(commentModel), Times.Once);
        }

        [Test]
        public void UpdateComment_ExceptionThrown_ThrowsCommentUpdateException()
        {
            // Arrange
            var commentModel = CommentModelFixture.Builder()
                .WithCommentText("Valid comment")
                .Build();

            _mockCommentRepository
                .Setup(repo => repo.Upsert(It.IsAny<CommentModel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CommentUpdateException>(() =>
                _updateCommentService.UpdateComment(commentModel)
            );
        }
    }
}
