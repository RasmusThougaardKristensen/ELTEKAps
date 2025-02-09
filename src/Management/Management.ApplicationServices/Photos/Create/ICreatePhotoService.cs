using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Photos;

namespace ELTEKAps.Management.ApplicationServices.Photos.Create
{
    public interface ICreatePhotoService
    {
        /// <summary>
        /// Queues an operation to create a new photo.
        /// </summary>
        /// <param name="photosModel">PhotosModel containing data to create.</param>
        /// <param name="operationDetails">Operation details such as who initiated it.</param>
        /// <returns>An OperationResult indicating the outcome.</returns>
        Task<OperationResult> RequestCreatePhoto(PhotoModel photosModel, OperationDetails operationDetails);

        /// <summary>
        /// Performs creation of a photo in the repository.
        /// </summary>
        /// <param name="photosModel">PhotosModel to persist in the repository.</param>
        Task CreatePhoto(PhotoModel photosModel);
    }
}
