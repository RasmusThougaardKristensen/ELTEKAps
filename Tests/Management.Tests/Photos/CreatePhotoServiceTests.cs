using ELTEKAps.Management.ApplicationServices.BlobService.CreateBlobBlock;
using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Photos;
using ELTEKAps.Management.ApplicationServices.Photos.Create;
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
    public class CreatePhotoServiceTests
    {
        private Mock<IPhotoRepository> _mockPhotoRepository;
        private Mock<IOperationService> _mockOperationService;
        private Mock<ILogger<CreatePhotoService>> _mockLogger;
        private Mock<ICreateBlobBlockService> _mockCreateBlobBlockService;
        private CreatePhotoService _createPhotoService;

        [SetUp]
        public void SetUp()
        {
            _mockPhotoRepository = new Mock<IPhotoRepository>();
            _mockOperationService = new Mock<IOperationService>();
            _mockLogger = new Mock<ILogger<CreatePhotoService>>();
            _mockCreateBlobBlockService = new Mock<ICreateBlobBlockService>();

            _createPhotoService = new CreatePhotoService(
                _mockPhotoRepository.Object,
                _mockOperationService.Object,
                _mockLogger.Object,
                _mockCreateBlobBlockService.Object
            );
        }

        [Test]
        public async Task RequestCreatePhoto_ValidData_ReturnsAcceptedOperationResult()
        {
            // Arrange
            var validBase64 = "dGVzdA==";
            var photoModel = PhotoModelFixture.Builder()
                .WithPhotoData(validBase64)
                .Build();
            var operationDetails = new OperationDetails("test-user");

            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoModel.Id))
                .ReturnsAsync((PhotoModel)null);

            var operation = OperationFixture.Builder()
                .WithOperationName(OperationName.CreatePhoto)
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ReturnsAsync(operation);

            _mockCreateBlobBlockService
                .Setup(s => s.CreateBlobBlock(validBase64))
                .ReturnsAsync("blob-uri");

            // Act
            var result = await _createPhotoService.RequestCreatePhoto(photoModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Accepted));
            _mockPhotoRepository.Verify(repo => repo.GetById(photoModel.Id), Times.Once);
            _mockCreateBlobBlockService.Verify(s => s.CreateBlobBlock(validBase64), Times.Once);
            _mockPhotoRepository.Verify(repo =>
                repo.Upsert(It.Is<PhotoModel>(p => p.Id == photoModel.Id && p.PhotoData == "blob-uri")),
                Times.Once);
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Once);
        }

        [Test]
        public async Task RequestCreatePhoto_InvalidData_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var invalidPhotoData = "";
            var photoModel = PhotoModelFixture.Builder()
                .WithPhotoData(invalidPhotoData)
                .Build();
            var operationDetails = new OperationDetails("test-user");

            // Act
            var result = await _createPhotoService.RequestCreatePhoto(photoModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockPhotoRepository.Verify(repo => repo.GetById(It.IsAny<Guid>()), Times.Never);
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
        }

        [Test]
        public async Task RequestCreatePhoto_PhotoAlreadyExists_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var validBase64 = "dGVzdA==";
            var photoModel = PhotoModelFixture.Builder()
                .WithPhotoData(validBase64)
                .Build();
            var operationDetails = new OperationDetails("test-user");

            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoModel.Id))
                .ReturnsAsync(photoModel);

            // Act
            var result = await _createPhotoService.RequestCreatePhoto(photoModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockPhotoRepository.Verify(repo => repo.Upsert(It.IsAny<PhotoModel>()), Times.Never);
        }

        [Test]
        public void RequestCreatePhoto_ExceptionThrown_ThrowsPhotoOperationException()
        {
            // Arrange
            var validBase64 = "dGVzdA==";
            var photoModel = PhotoModelFixture.Builder()
                .WithPhotoData(validBase64)
                .Build();
            var operationDetails = new OperationDetails("test-user");

            // Ensure that the photo does not already exist.
            _mockPhotoRepository
                .Setup(repo => repo.GetById(photoModel.Id))
                .ReturnsAsync((PhotoModel)null);

            // Simulate an exception when queuing the operation (inside the try block).
            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<PhotoOperationException>(() =>
                _createPhotoService.RequestCreatePhoto(photoModel, operationDetails)
            );
        }

        [Test]
        public async Task CreatePhoto_ValidPhoto_CreatesPhotoSuccessfully()
        {
            // Arrange
            var validBase64 = "dGVzdA==";
            var photoModel = PhotoModelFixture.Builder()
                .WithPhotoData(validBase64)
                .Build();

            _mockCreateBlobBlockService
                .Setup(s => s.CreateBlobBlock(validBase64))
                .ReturnsAsync("new-blob-uri");

            // Act
            await _createPhotoService.CreatePhoto(photoModel);

            // Assert
            _mockCreateBlobBlockService.Verify(s => s.CreateBlobBlock(validBase64), Times.Once);
            _mockPhotoRepository.Verify(repo =>
                repo.Upsert(It.Is<PhotoModel>(p => p.Id == photoModel.Id && p.PhotoData == "new-blob-uri")),
                Times.Once);
        }

        [Test]
        public void CreatePhoto_ExceptionThrown_ThrowsPhotoCreationException()
        {
            // Arrange
            var validBase64 = "dGVzdA==";
            var photoModel = PhotoModelFixture.Builder()
                .WithPhotoData(validBase64)
                .Build();

            _mockCreateBlobBlockService
                .Setup(s => s.CreateBlobBlock(validBase64))
                .ReturnsAsync("new-blob-uri");

            _mockPhotoRepository
                .Setup(repo => repo.Upsert(It.IsAny<PhotoModel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<PhotoCreationException>(() =>
                _createPhotoService.CreatePhoto(photoModel)
            );
        }
    }
}
