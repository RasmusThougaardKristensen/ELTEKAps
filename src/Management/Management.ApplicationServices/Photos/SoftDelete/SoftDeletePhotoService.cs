using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Photos;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Photos.SoftDelete
{
    public class SoftDeletePhotoService : ISoftDeletePhotoService
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<SoftDeletePhotoService> _logger;

        public SoftDeletePhotoService(
            IPhotoRepository photoRepository,
            IOperationService operationService,
            ILogger<SoftDeletePhotoService> logger)
        {
            _photoRepository = photoRepository;
            _operationService = operationService;
            _logger = logger;
        }

        public async Task<OperationResult> RequestSoftDeletePhoto(Guid photoId, OperationDetails operationDetails)
        {
            try
            {
                _logger.LogInformation("Request soft-delete for Photo with ID: {PhotoId}", photoId);

                var existingPhoto = await _photoRepository.GetById(photoId);
                if (existingPhoto == null)
                {
                    _logger.LogWarning("Photo with ID: {PhotoId} does not exist", photoId);
                    return OperationResult.InvalidState("Photo does not exist.");
                }

                // If the photo is already soft-deleted, return an invalid state.
                if (existingPhoto.Deleted)
                {
                    _logger.LogWarning("Photo with ID: {PhotoId} is already soft-deleted", photoId);
                    return OperationResult.InvalidState("Photo is already soft-deleted.");
                }

                // Queue the soft-delete operation
                var operation = await _operationService.QueueOperation(
                    OperationBuilder.SoftDeletePhoto(photoId, operationDetails.CreatedBy)
                );

                _logger.LogInformation("Operation queued with Request ID: {RequestId} for Photo ID: {PhotoId}",
                    operation.RequestId, photoId);

                await SoftDeletePhoto(photoId);

                return OperationResult.Accepted(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during soft-delete request for Photo with ID: {PhotoId}", photoId);
                throw new PhotoOperationException("Failed to queue and soft-delete the photo.", ex);
            }
        }

        public async Task SoftDeletePhoto(Guid photoId)
        {
            try
            {
                _logger.LogInformation("Soft-deleting Photo with ID: {PhotoId}", photoId);

                var photo = await _photoRepository.GetById(photoId);
                if (photo == null)
                {
                    _logger.LogWarning("Photo with ID: {PhotoId} was not found for soft-delete", photoId);
                    return;
                }

                photo.SoftDelete();
                await _photoRepository.Upsert(photo);

                _logger.LogInformation("Photo with ID: {PhotoId} has been soft-deleted", photoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft-deleting Photo with ID: {PhotoId}", photoId);
                throw new PhotoSoftDeleteException("Failed to soft-delete the photo.", ex);
            }
        }
    }
}
