using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.Photos.SoftDelete;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Photos.SoftDeletePhoto
{
    [Authorize]
    public class SoftDeletePhotoEndpoint : EndpointBaseAsync
        .WithRequest<DeletePhotoRequest>
        .WithoutResult
    {
        private readonly ISoftDeletePhotoService _softDeletePhotoService;

        public SoftDeletePhotoEndpoint(ISoftDeletePhotoService softDeletePhotoService)
        {
            _softDeletePhotoService = softDeletePhotoService;
        }

        [HttpDelete("api/photos/{photoId:guid}")]
        [ProducesResponseType(typeof(OperationAcceptedResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Soft Delete Photo",
            Description = "Soft delete a photo by ID (mark as deleted without removing data)",
            OperationId = "SoftDeletePhoto",
            Tags = new[] { "Photo" })
        ]
        public override async Task<ActionResult> HandleAsync(
            [FromRoute] DeletePhotoRequest request,
            CancellationToken cancellationToken = default)
        {
            var operationResult = await _softDeletePhotoService.RequestSoftDeletePhoto(
                request.photoId,
                new OperationDetails(Guid.NewGuid().ToString())
            );

            return operationResult.Status switch
            {
                OperationResultStatus.Accepted => new AcceptedResult(
                    new Uri($"/api/operations/{operationResult.GetOperation().RequestId}", UriKind.Relative),
                    new OperationAcceptedResponse(operationResult.GetOperation().RequestId)
                ),
                OperationResultStatus.InvalidState => Problem(
                    title: "Cannot soft delete photo",
                    detail: operationResult.GetMessage(),
                    statusCode: StatusCodes.Status400BadRequest
                ),
                _ => Problem(
                    title: "Unknown error requesting to soft delete photo",
                    detail: "Unknown error - check logs",
                    statusCode: StatusCodes.Status500InternalServerError
                )
            };
        }
    }

    public class DeletePhotoRequest
    {
        [FromRoute(Name = "photoId")]
        public Guid photoId { get; set; }
    }
}
