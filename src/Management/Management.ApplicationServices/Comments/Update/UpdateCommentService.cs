using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Comments;
using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Comments.Update
{
    public class UpdateCommentService : IUpdateCommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<UpdateCommentService> _logger;

        public UpdateCommentService(
            ICommentRepository commentRepository,
            IOperationService operationService,
            ILogger<UpdateCommentService> logger)
        {
            _commentRepository = commentRepository;
            _operationService = operationService;
            _logger = logger;
        }

        public async Task<OperationResult> RequestUpdateComment(CommentModel commentModel, OperationDetails operationDetails)
        {
            try
            {
                _logger.LogInformation("Request to update Comment with ID: {CommentId}", commentModel.Id);

                if (!IsValidComment(commentModel))
                {
                    return OperationResult.InvalidState("The provided comment is invalid.");
                }

                var existingComment = await _commentRepository.GetById(commentModel.Id);
                if (existingComment == null)
                {
                    _logger.LogWarning("Comment with ID: {CommentId} does not exist", commentModel.Id);
                    return OperationResult.InvalidState("Comment does not exist.");
                }

                var operation = await _operationService.QueueOperation(
                    OperationBuilder.UpdateComment(commentModel, operationDetails.CreatedBy)
                );

                _logger.LogInformation(
                    "Operation queued with Request ID: {RequestId} for Comment ID: {CommentId}",
                    operation.RequestId,
                    commentModel.Id
                );

                await UpdateComment(commentModel);

                return OperationResult.Accepted(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during comment update operation for Comment ID: {CommentId}", commentModel.Id);
                throw new CommentOperationException("Failed to queue and update comment.", ex);
            }
        }

        public async Task UpdateComment(CommentModel commentModel)
        {
            try
            {
                _logger.LogInformation("Updating Comment with ID: {CommentId}", commentModel.Id);
                await _commentRepository.Upsert(commentModel);
                _logger.LogInformation("Comment updated with ID: {CommentId}", commentModel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment with ID: {CommentId}", commentModel.Id);
                throw new CommentUpdateException("Failed to update comment.", ex);
            }
        }

        private bool IsValidComment(CommentModel commentModel)
        {
            return !string.IsNullOrWhiteSpace(commentModel.Comment);
        }
    }
}
