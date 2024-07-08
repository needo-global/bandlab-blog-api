using System.ComponentModel.DataAnnotations;

namespace Posts.Api.Models.Request;

public class CreatePostCommentRequest
{
    [Required(ErrorMessage = "The content is required")]
    public string Content { get; set; }
}