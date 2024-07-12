using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Posts.Api.Controllers;
using Posts.Api.Models;
using Posts.Domain.Abstract;
using Posts.Domain.Exceptions;

namespace Posts.Api.Test;

public class PostControllerTest
{
    private readonly Mock<IPostService> _postService = new();
    private readonly PostController _postController;

    public PostControllerTest()
    {
        _postController = new PostController(_postService.Object);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(null, "")]
    [InlineData("postId", null)]
    [InlineData(null, "commentId")]
    public async Task Should_Return_BadRequest_When_Try_To_Delete_Comment(string postId, string commentId)
    {
        // Arrange
        
        // Act
        var result = await _postController.DeletePostComment(postId, commentId);

        // Assert
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult.StatusCode.Should().Be(400);

        var errorData = badResult.Value as ErrorDto;
        errorData.Should().NotBeNull();
        errorData.Code.Should().Be("E001");
    }

    [Fact]
    public async Task Should_Return_UnauthorizedAccess_When_Try_To_Delete_Comment()
    {
        // Arrange
        _postService.Setup(p => p.DeletePostCommentAsync("ranganapeiris", "postId", "commentId"))
            .ThrowsAsync(new UnauthorizedAccessException("Not the owner of the comment"));

        // Act
        var result = await _postController.DeletePostComment("postId", "commentId");

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult.StatusCode.Should().Be(401);

        var errorData = unauthorizedResult.Value as ErrorDto;
        errorData.Should().NotBeNull();
        errorData.Code.Should().Be("E003");
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Try_To_Delete_Comment()
    {
        // Arrange
        _postService.Setup(p => p.DeletePostCommentAsync("ranganapeiris", "postId", "commentId"))
            .ThrowsAsync(new NotFoundException("comment not found"));

        // Act
        var result = await _postController.DeletePostComment("postId", "commentId");

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);

        var errorData = notFoundResult.Value as ErrorDto;
        errorData.Should().NotBeNull();
        errorData.Code.Should().Be("E002");
    }

    [Fact]
    public async Task Should_Return_Ok_When_Try_To_Delete_Comment()
    {
        // Arrange
        _postService.Setup(p => p.DeletePostCommentAsync("ranganapeiris", "postId", "commentId"));

        // Act
        var result = await _postController.DeletePostComment("postId", "commentId");

        // Assert
        var okResult = result as OkResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
    }
}