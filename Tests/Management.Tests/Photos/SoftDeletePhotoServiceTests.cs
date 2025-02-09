using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Photos;
using ELTEKAps.Management.ApplicationServices.Photos.SoftDelete;
using ELTEKAps.Management.ApplicationServices.Repositories.Photos;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Photos;
using ELTEKAps.Management.TestFixtures.Operations;
using ELTEKAps.Management.TestFixtures.Photos;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Photos
{
    [TestFixture]
    public class SoftDeletePhotoServiceTests
    {
        private Mock<IPhotoRepository> _mockPhotoRepository;
        private Mock<IOperationService> _mockOperationService;
        private Mock<ILogger<SoftDeletePhotoService>> _mockLogger;
        private SoftDeletePhotoService _softDeletePhotoService;

        [SetUp]
        public void SetUp()
        {
            _mockPhotoRepository = new Mock<IPhotoRepository>();
            _mockOperationService = new Mock<IOperationService>();
            _mockLogger = new Mock<ILogger<SoftDeletePhotoService>>();

            _softDeletePhotoService = new SoftDeletePhotoService(
                _mockPhotoRepository.Object,
                _mockOperationService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task RequestSoftDeletePhoto_ValidPhoto_ReturnsAcceptedOperationResult()
        {
            // Arrange
            var photoId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");
            var existingPhoto = PhotoModelFixture.Builder()
                .WithId(photoId)
                .WithDeleted(false)
                .Build();

            var operation = OperationFixture.Builder()
                .WithOperationName(OperationName.SoftDeletePhoto)
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoId))
                .ReturnsAsync(existingPhoto);

            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ReturnsAsync(operation);

            // Act
            var result = await _softDeletePhotoService.RequestSoftDeletePhoto(photoId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Accepted));
            Assert.That(result.GetOperation().RequestId, Is.EqualTo(operation.RequestId));

            // The repository should be called twice: once to check existence and once inside SoftDeletePhoto.
            _mockPhotoRepository.Verify(repo => repo.GetById(photoId), Times.Exactly(2));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Once);
            _mockPhotoRepository.Verify(repo =>
                repo.Upsert(It.Is<PhotoModel>(p => p.Id == photoId && p.Deleted)),
                Times.Once);
        }

        [Test]
        public async Task RequestSoftDeletePhoto_PhotoDoesNotExist_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var photoId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");

            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoId))
                .ReturnsAsync((PhotoModel)null);

            // Act
            var result = await _softDeletePhotoService.RequestSoftDeletePhoto(photoId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            Assert.That(result.GetMessage(), Is.EqualTo("Photo does not exist."));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockPhotoRepository.Verify(repo => repo.Upsert(It.IsAny<PhotoModel>()), Times.Never);
        }

        [Test]
        public async Task RequestSoftDeletePhoto_PhotoAlreadySoftDeleted_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var photoId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");
            var existingPhoto = PhotoModelFixture.Builder()
                .WithId(photoId)
                .WithDeleted(true)
                .Build();

            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoId))
                .ReturnsAsync(existingPhoto);

            // Act
            var result = await _softDeletePhotoService.RequestSoftDeletePhoto(photoId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            Assert.That(result.GetMessage(), Is.EqualTo("Photo is already soft-deleted."));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockPhotoRepository.Verify(repo => repo.Upsert(It.IsAny<PhotoModel>()), Times.Never);
        }

        [Test]
        public void RequestSoftDeletePhoto_ExceptionThrown_ThrowsPhotoOperationException()
        {
            // Arrange
            var photoId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");

            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<PhotoOperationException>(() =>
                _softDeletePhotoService.RequestSoftDeletePhoto(photoId, operationDetails)
            );
        }

        [Test]
        public async Task SoftDeletePhoto_ValidPhoto_UpsertsSoftDeletedPhoto()
        {
            // Arrange
            var photoId = Guid.NewGuid();
            var existingPhoto = PhotoModelFixture.Builder()
                .WithId(photoId)
                .WithDeleted(false)
                .Build();

            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoId))
                .ReturnsAsync(existingPhoto);

            // Act
            await _softDeletePhotoService.SoftDeletePhoto(photoId);

            // Assert
            _mockPhotoRepository.Verify(repo => repo.GetById(photoId), Times.Once);
            _mockPhotoRepository.Verify(repo =>
                repo.Upsert(It.Is<PhotoModel>(p => p.Id == photoId && p.Deleted)),
                Times.Once);
        }

        [Test]
        public async Task SoftDeletePhoto_PhotoDoesNotExist_DoesNotCallUpsert()
        {
            // Arrange
            var photoId = Guid.NewGuid();
            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoId))
                .ReturnsAsync((PhotoModel)null);

            // Act
            await _softDeletePhotoService.SoftDeletePhoto(photoId);

            // Assert
            _mockPhotoRepository.Verify(repo => repo.GetById(photoId), Times.Once);
            _mockPhotoRepository.Verify(repo => repo.Upsert(It.IsAny<PhotoModel>()), Times.Never);
        }

        [Test]
        public void SoftDeletePhoto_ExceptionThrown_ThrowsPhotoSoftDeleteException()
        {
            // Arrange
            var photoId = Guid.NewGuid();
            var existingPhoto = PhotoModelFixture.Builder()
                .WithId(photoId)
                .WithDeleted(false)
                .Build();

            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoId))
                .ReturnsAsync(existingPhoto);
            _mockPhotoRepository
                .Setup(repo => repo.Upsert(It.IsAny<PhotoModel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<PhotoSoftDeleteException>(() =>
                _softDeletePhotoService.SoftDeletePhoto(photoId)
            );
        }
    }
}
