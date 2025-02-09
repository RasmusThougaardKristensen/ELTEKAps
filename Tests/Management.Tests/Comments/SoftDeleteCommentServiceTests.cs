using ELTEKAps.Management.ApplicationServices.Comments;
using ELTEKAps.Management.ApplicationServices.Comments.Delete;
using ELTEKAps.Management.ApplicationServices.Comments.SoftDelete;
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
    public class SoftDeleteCommentServiceTests
    {
        private Mock<ICommentRepository> _mockCommentRepository;
        private Mock<IOperationService> _mockOperationService;
        private Mock<ILogger<SoftDeleteCommentService>> _mockLogger;
        private SoftDeleteCommentService _softDeleteCommentService;

        [SetUp]
        public void SetUp()
        {
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockOperationService = new Mock<IOperationService>();
            _mockLogger = new Mock<ILogger<SoftDeleteCommentService>>();
            _softDeleteCommentService = new SoftDeleteCommentService(
                _mockCommentRepository.Object,
                _mockOperationService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task RequestSoftDeleteComment_ValidCommentId_ReturnsAcceptedOperationResult()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");
            var comment = CommentModelFixture.Builder()
                .WithId(commentId)
                .Build();
            var operation = OperationFixture.Builder()
                .WithOperationName(OperationName.SoftDeleteComment)
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockCommentRepository
                .Setup(repo => repo.GetById(commentId))
                .ReturnsAsync(comment);

            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ReturnsAsync(operation);

            // Act
            var result = await _softDeleteCommentService.RequestSoftDeleteComment(commentId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Accepted));
            _mockCommentRepository.Verify(repo => repo.GetById(commentId), Times.Exactly(2)); // Updated to expect 2 calls
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Once);
            _mockCommentRepository.Verify(repo => repo.Upsert(It.Is<CommentModel>(c => c.Deleted)), Times.Once);
        }

        [Test]
        public async Task RequestSoftDeleteComment_CommentDoesNotExist_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");

            _mockCommentRepository
                .Setup(repo => repo.GetById(commentId))
                .ReturnsAsync((CommentModel)null);

            // Act
            var result = await _softDeleteCommentService.RequestSoftDeleteComment(commentId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockCommentRepository.Verify(repo => repo.GetById(commentId), Times.Once);
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCommentRepository.Verify(repo => repo.Upsert(It.IsAny<CommentModel>()), Times.Never);
        }

        [Test]
        public void RequestSoftDeleteComment_ExceptionThrown_ThrowsCommentOperationException()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");

            _mockCommentRepository
                .Setup(repo => repo.GetById(commentId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CommentOperationException>(() =>
                _softDeleteCommentService.RequestSoftDeleteComment(commentId, operationDetails)
            );
        }

        [Test]
        public async Task SoftDeleteComment_ValidCommentId_SoftDeletesCommentSuccessfully()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var comment = CommentModelFixture.Builder()
                .WithId(commentId)
                .Build();

            _mockCommentRepository
                .Setup(repo => repo.GetById(commentId))
                .ReturnsAsync(comment);

            // Act
            await _softDeleteCommentService.SoftDeleteComment(commentId);

            // Assert
            _mockCommentRepository.Verify(repo => repo.GetById(commentId), Times.Once);
            _mockCommentRepository.Verify(repo => repo.Upsert(It.Is<CommentModel>(c => c.Deleted)), Times.Once);
        }

        [Test]
        public async Task SoftDeleteComment_CommentDoesNotExist_LogsWarningAndDoesNotUpsert()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            _mockCommentRepository
                .Setup(repo => repo.GetById(commentId))
                .ReturnsAsync((CommentModel)null);

            // Act
            await _softDeleteCommentService.SoftDeleteComment(commentId);

            // Assert
            _mockCommentRepository.Verify(repo => repo.GetById(commentId), Times.Once);
            _mockCommentRepository.Verify(repo => repo.Upsert(It.IsAny<CommentModel>()), Times.Never);
        }

        [Test]
        public void SoftDeleteComment_ExceptionThrown_ThrowsCommentSoftDeleteException()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var comment = CommentModelFixture.Builder()
                .WithId(commentId)
                .Build();

            _mockCommentRepository
                .Setup(repo => repo.GetById(commentId))
                .ReturnsAsync(comment);

            _mockCommentRepository
                .Setup(repo => repo.Upsert(It.IsAny<CommentModel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CommentSoftDeleteException>(() =>
                _softDeleteCommentService.SoftDeleteComment(commentId)
            );
        }
    }
}
