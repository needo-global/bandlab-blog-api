using System.ComponentModel.DataAnnotations;

namespace Posts.Api.Models.Request;

public class CreatePostCommentRequest
{
    [Required(ErrorMessage = "The content is required")]
    [MaxLength(300, ErrorMessage = "The content is too long")]
    public string Content { get; set; }
}