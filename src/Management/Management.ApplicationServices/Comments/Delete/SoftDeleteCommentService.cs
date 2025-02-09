using ELTEKAps.Management.ApplicationServices.Comments.Delete;
using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Comments;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Comments.SoftDelete
{
    public class SoftDeleteCommentService : ISoftDeleteCommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<SoftDeleteCommentService> _logger;

        public SoftDeleteCommentService(
            ICommentRepository commentRepository,
            IOperationService operationService,
            ILogger<SoftDeleteCommentService> logger)
        {
            _commentRepository = commentRepository;
            _operationService = operationService;
            _logger = logger;
        }

        public async Task<OperationResult> RequestSoftDeleteComment(Guid commentId, OperationDetails operationDetails)
        {
            try
            {
                _logger.LogInformation("Request soft-delete for Comment with ID: {CommentId}", commentId);

                var existingComment = await _commentRepository.GetById(commentId);
                if (existingComment == null)
                {
                    _logger.LogWarning("Comment with ID: {CommentId} does not exist", commentId);
                    return OperationResult.InvalidState("Comment does not exist");
                }

                var operation = await _operationService.QueueOperation(
                    OperationBuilder.SoftDeleteComment(commentId, operationDetails.CreatedBy)
                );

                _logger.LogInformation("Operation queued with Request ID: {RequestId} for Comment ID: {CommentId}", operation.RequestId, commentId);

                await SoftDeleteComment(commentId);

                return OperationResult.Accepted(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during soft-delete request for Comment ID: {CommentId}", commentId);
                throw new CommentOperationException("Failed to queue and soft-delete comment.", ex);
            }
        }

        public async Task SoftDeleteComment(Guid commentId)
        {
            try
            {
                _logger.LogInformation("Performing soft-delete for Comment with ID: {CommentId}", commentId);

                var comment = await _commentRepository.GetById(commentId);
                if (comment == null)
                {
                    _logger.LogWarning("Comment with ID: {CommentId} not found for soft-delete", commentId);
                    return;
                }

                comment.SoftDelete();
                await _commentRepository.Upsert(comment);

                _logger.LogInformation("Comment with ID: {CommentId} has been soft-deleted", commentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft-deleting Comment with ID: {CommentId}", commentId);
                throw new CommentSoftDeleteException("Failed to soft-delete comment.", ex);
            }
        }
    }
}
