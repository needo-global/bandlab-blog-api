using Microsoft.AspNetCore.Mvc;
using Posts.Api.Models.Request;
using System.Net;
using Posts.Api.Models;
using Posts.Domain.Abstract;
using Posts.Domain.Exceptions;
using Posts.Api.Models.Response;
using Posts.Api.Middleware.Validation;

namespace Posts.Api.Controllers;

[Route("posts")]
[ApiController]
// [Authorize(AuthenticationSchemes = "Bearer")] - TODO - Add authentication and authorization using a bearer token
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Post([FromForm] CreatePostRequest? request)
    {
        await using var memoryStream = new MemoryStream();
        await request.Image.CopyToAsync(memoryStream);

        var userId = "ranganapeiris"; // TODO - This should be obtained from token claims
        var postId = await _postService.AddPostAsync(userId, request.Caption, request.Filename, memoryStream.ToArray());

        return CreatedAtRoute("GetPostById", new {postId}, new { Id = postId });
    }

    [HttpPost("{postId}/comment")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> PostComment([FromRoute] string postId, [FromBody] CreatePostCommentRequest request)
    {
        if (string.IsNullOrWhiteSpace(postId))
            return new BadRequestObjectResult(new ErrorDto("E001", "Invalid post id"));
       
        try
        {
            var userId = "ranganapeiris"; // TODO - This should be obtained from token claims
            var commentId = await _postService.PostCommentAsync(userId, postId, request.Content);
            return CreatedAtRoute("GetPostById", new { postId }, new { Id = commentId });
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
            await _postService.DeletePostCommentAsync(userId, postId, commentId);
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
    public async Task<IActionResult> Get([FromQuery] string? lastPostToken)
    {
        var posts = await _postService.GetPostsByPaging(lastPostToken);

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
}