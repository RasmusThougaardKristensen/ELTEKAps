using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Comments;
using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Comments.Create
{
    public class CreateCommentService : ICreateCommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<CreateCommentService> _logger;

        public CreateCommentService(
            ICommentRepository commentRepository,
            IOperationService operationService,
            ILogger<CreateCommentService> logger)
        {
            _commentRepository = commentRepository;
            _operationService = operationService;
            _logger = logger;
        }

        public async Task<OperationResult> RequestCreateComment(CommentModel commentModel, OperationDetails operationDetails)
        {
            try
            {
                _logger.LogInformation("Request to create Comment with ID: {CommentId}", commentModel.Id);

                if (!IsValidComment(commentModel))
                {
                    return OperationResult.InvalidState("The provided comment is invalid.");
                }

                var operation = await _operationService.QueueOperation(
                    OperationBuilder.CreateComment(commentModel, operationDetails.CreatedBy)
                );

                _logger.LogInformation("Operation queued with Request ID: {RequestId} for Comment ID: {CommentId}", operation.RequestId, commentModel.Id);

                await CreateComment(commentModel);

                return OperationResult.Accepted(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during comment creation for Comment ID: {CommentId}", commentModel.Id);
                throw new CommentOperationException("Failed to queue and create comment.", ex);
            }
        }

        public async Task CreateComment(CommentModel commentModel)
        {
            try
            {
                _logger.LogInformation("Creating Comment with ID: {CommentId}", commentModel.Id);
                await _commentRepository.Upsert(commentModel);
                _logger.LogInformation("Comment created with ID: {CommentId}", commentModel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment with ID: {CommentId}", commentModel.Id);
                throw new CommentCreationException("Failed to create comment.", ex);
            }
        }

        private bool IsValidComment(CommentModel commentModel)
        {
            return !string.IsNullOrWhiteSpace(commentModel.Comment);
        }
    }
}
