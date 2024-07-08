using Microsoft.AspNetCore.Mvc;
using Posts.Api.Models.Request;
using System.Net;
using Posts.Api.Models;
using Posts.Domain.Abstract;
using Posts.Domain.Exceptions;

namespace Posts.Api.Controllers;

[RequireHttps]
[Route("posts")]
[ApiController]
// [Authorize(AuthenticationSchemes = "Bearer")] - TODO - Add authentication and authorization using a bearer token
public class PostController(IPostService postService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Post([FromForm] CreatePostRequest? request)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{postId}/comment")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> PostComment([FromRoute] string postId, [FromBody] CreatePostCommentRequest request)
    {
        if (string.IsNullOrWhiteSpace(postId))
            return new BadRequestObjectResult(new ErrorDto("E001", "Invalid post id"));

        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(new ErrorDto("E001",
                ModelState[nameof(CreatePostCommentRequest.Content)]?.Errors?.FirstOrDefault().ErrorMessage));
        }

        try
        {
            var userId = "ranganapeiris"; // TODO - This should be obtained from token claims
            await postService.PostCommentAsync(userId, postId, request.Content);
            return Created(); // NOTE: Location of the created resource not specified here, because we don't have route to get the comment directly for this exercise
        }
        catch (NotFoundException e)
        {
            // TODO - Logging
            return new NotFoundObjectResult(new ErrorDto("E002", e.Message));
        }
    }

    [HttpDelete("{postId}/comment/{commentId}")]
    // [Authorize(Policy = "CommentOwner")] - TODO - Add authorization policy to validate logged in user is the owner of the comment
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeletePostComment([FromRoute] string postId, [FromRoute] string commentId)
    {
        if (string.IsNullOrWhiteSpace(postId))
            return new BadRequestObjectResult(new ErrorDto("E001", "Invalid post id"));

        if (string.IsNullOrWhiteSpace(commentId))
            return new BadRequestObjectResult(new ErrorDto("E001", "Invalid comment id"));

        try
        {
            var userId = "ranganapeiris"; // TODO - This should be obtained from token claims
            await postService.DeletePostCommentAsync(userId, postId, commentId);
            return Ok();
        }
        catch (UnauthorizedAccessException e) // TODO - Remove once the authorization policy is added to the action method
        {
            // TODO - Logging
            return new UnauthorizedObjectResult(new ErrorDto("E003", e.Message));
        }
        catch (NotFoundException e)
        {
            // TODO - Logging
            return new NotFoundObjectResult(new ErrorDto("E002", e.Message));
        }
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] string lastPostToken)
    {
        throw new NotImplementedException();
    }
}