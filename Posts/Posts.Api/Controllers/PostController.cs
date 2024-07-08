using Microsoft.AspNetCore.Mvc;
using Posts.Api.Models.Request;
using System.Net;
using Posts.Api.Models;

namespace Posts.Api.Controllers
{
    [Route("posts")]
    [ApiController]
    public class PostController : ControllerBase
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
        [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostComment([FromRoute] string postId, [FromBody] CreatePostCommentRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("comment/{commentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            throw new NotImplementedException();
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
}
