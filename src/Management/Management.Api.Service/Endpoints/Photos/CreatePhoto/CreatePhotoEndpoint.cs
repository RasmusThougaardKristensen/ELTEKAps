using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Endpoints.Tasks.CreateTask;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.Photos.Create;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Photos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Photos.CreatePhoto
{
    [Authorize]
    public class CreatePhotoEndpoint : EndpointBaseAsync
        .WithRequest<CreatePhotoRequest>
        .WithoutResult
    {
        private readonly ICreatePhotoService _createPhotoService;

        public CreatePhotoEndpoint(ICreatePhotoService createPhotoService)
        {
            _createPhotoService = createPhotoService;
        }

        [HttpPost("api/tasks/{taskId:guid}/photos")]
        [ProducesResponseType(typeof(OperationAcceptedResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Create Photo",
            Description = "Create a new photo and associate it with a task",
            OperationId = "CreatePhoto",
            Tags = new[] { "Photo" })
        ]
        public override async Task<ActionResult> HandleAsync(
            [FromRoute] CreatePhotoRequest request,
            CancellationToken cancellationToken = default)
        {
            // Create the PhotoModel
            var photoModel = PhotoModel.Create(request.Details.Photo, request.TaskId);

            if (photoModel == null)
            {
                return Problem(
                    title: "Invalid Photo",
                    detail: "Photo cannot be empty",
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            // Request to create the photo
            var operationResult = await _createPhotoService.RequestCreatePhoto(
                photoModel,
                new OperationDetails(Guid.NewGuid().ToString())
            );

            return operationResult.Status switch
            {
                OperationResultStatus.Accepted => new AcceptedResult(
                    new Uri($"/api/operations/{operationResult.GetOperation().RequestId}", UriKind.Relative),
                    new OperationAcceptedResponse(operationResult.GetOperation().RequestId)
                ),
                OperationResultStatus.InvalidState => Problem(
                    title: "Cannot create photo",
                    detail: operationResult.GetMessage(),
                    statusCode: StatusCodes.Status400BadRequest
                ),
                _ => Problem(
                    title: "Unknown error requesting to create photo",
                    detail: "Unknown error - check logs",
                    statusCode: StatusCodes.Status500InternalServerError
                )
            };
        }
    }

    public class CreatePhotoRequest : TaskRequest<CreatePhotoDetails>
    {
        [FromRoute(Name = "taskId")]
        public Guid TaskId { get; set; }
    }

    public class CreatePhotoDetails
    {
        public string Photo { get; set; }
    }
}
