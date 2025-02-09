using ELTEKAps.Management.ApplicationServices.BlobService.CreateBlobBlock;
using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Photos;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Photos;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Photos.Create
{
    public class CreatePhotoService : ICreatePhotoService
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<CreatePhotoService> _logger;
        private readonly ICreateBlobBlockService _createBlobBlockService;

        public CreatePhotoService(
            IPhotoRepository photoRepository,
            IOperationService operationService,
            ILogger<CreatePhotoService> logger,
            ICreateBlobBlockService createBlobBlockService)
        {
            _photoRepository = photoRepository;
            _operationService = operationService;
            _logger = logger;
            _createBlobBlockService = createBlobBlockService;
        }

        public async Task<OperationResult> RequestCreatePhoto(PhotoModel photoModel, OperationDetails operationDetails)
        {
            _logger.LogInformation("Request to create new Photo with ID: {PhotoId}", photoModel.Id);

            // Validate photo model
            var validationErrors = ValidatePhoto(photoModel).ToList();
            if (validationErrors.Any())
            {
                var errorMessage = string.Join("; ", validationErrors);
                _logger.LogWarning("Validation failed for Photo with ID: {PhotoId}. Errors: {Errors}", photoModel.Id, errorMessage);
                return OperationResult.InvalidState(errorMessage);
            }

            // Optionally, check if the photo already exists
            var existingPhoto = await _photoRepository.GetById(photoModel.Id);
            if (existingPhoto != null)
            {
                _logger.LogWarning("Photo with ID: {PhotoId} already exists", photoModel.Id);
                return OperationResult.InvalidState("Photo already exists");
            }

            try
            {
                // Queue the operation
                var operation = await _operationService.QueueOperation(
                    OperationBuilder.CreatePhoto(photoModel, operationDetails.CreatedBy)
                );
                _logger.LogInformation("Operation queued with Request ID: {RequestId} for Photo ID: {PhotoId}",
                    operation.RequestId, photoModel.Id);

                await CreatePhoto(photoModel);
                return OperationResult.Accepted(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during photo creation for Photo ID: {PhotoId}", photoModel.Id);
                throw new PhotoOperationException("Failed to queue and create photo.", ex);
            }
        }

        public async Task CreatePhoto(PhotoModel photoModel)
        {
            _logger.LogInformation("Creating photo with ID: {PhotoId}", photoModel.Id);
            try
            {
                // Create the blob block and update the photo model with the blob URI.
                var photoUri = await _createBlobBlockService.CreateBlobBlock(photoModel.PhotoData);
                photoModel.PhotoData = photoUri;

                await _photoRepository.Upsert(photoModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating photo with ID: {PhotoId}", photoModel.Id);
                throw new PhotoCreationException("Failed to create photo.", ex);
            }

            _logger.LogInformation("Photo with ID: {PhotoId} has been created", photoModel.Id);
        }

        private IEnumerable<string> ValidatePhoto(PhotoModel photoModel)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(photoModel.PhotoData))
            {
                errors.Add("Photo data must be provided.");
            }

            return errors;
        }
    }
}
