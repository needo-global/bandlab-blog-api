using Microsoft.AspNetCore.Mvc;
using Posts.Api.Models.Request;
using System.Net;
using Posts.Api.Models;
using Posts.Domain.Abstract;
using Posts.Domain.Exceptions;
using Posts.Api.Models.Response;

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
        if (!ModelState.IsValid) return EmitValidationResult();

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Filename)}";
        await using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);

        var userId = "ranganapeiris"; // TODO - This should be obtained from token claims
        var postId = await postService.AddPostAsync(userId, request.Caption, fileName, memoryStream.ToArray());

        return CreatedAtRoute("GetPostById", new {postId}, new { Id = 1 });
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

        if (!ModelState.IsValid) return EmitValidationResult();
        
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
        var posts = await postService.GetPostsByPaging(lastPostToken);

        // TODO - Use Automapper if required
        var mapped = posts.Select(p => new PostDto
        {
            Id = p.Id,
            Image = p.Image,
            Caption = p.Caption,
            Creator = p.Creator,
            CreatedAt = p.CreatedAt,
            Comments = p.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                Creator = c.Creator,
                CreatedAt = c.CreatedAt,
            }).ToList()
        }).ToList();
        return new OkObjectResult(new GetPostsResponse(mapped));
    }

    [HttpGet("{postId}", Name = "GetPostById")]
    public async Task<IActionResult> GetPostById(string postId)
    {
        throw new NotImplementedException(); // NOTE: NOT Required for this exercise
    }

    private BadRequestObjectResult EmitValidationResult()
    {
        var errors = (from modelState in ModelState.Values from error in modelState.Errors select new ErrorDto("E001", error.ErrorMessage)).ToList();
        return new BadRequestObjectResult(new ErrorsDto(errors));
    }
}