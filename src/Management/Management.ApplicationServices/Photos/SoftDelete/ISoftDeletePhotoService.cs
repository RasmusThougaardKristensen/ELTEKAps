using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Photos.SoftDelete
{
    public interface ISoftDeletePhotoService
    {
        /// <summary>
        /// Queues an operation to soft-delete a photo.
        /// </summary>
        /// <param name="photoId">The ID of the photo to delete.</param>
        /// <param name="operationDetails">Operation details such as who initiated it.</param>
        /// <returns>An OperationResult indicating the outcome.</returns>
        Task<OperationResult> RequestSoftDeletePhoto(Guid photoId, OperationDetails operationDetails);

        /// <summary>
        /// Actually performs the soft-delete in the repository.
        /// </summary>
        /// <param name="photoId">The ID of the photo to delete.</param>
        Task SoftDeletePhoto(Guid photoId);
    }
}
