using System.ComponentModel.DataAnnotations;

namespace Posts.Api.Models.Request
{
    public class CreatePostRequest : IValidatableObject
    {
        public IFormFile? File { get; set; }
        public string Filename => File?.FileName ?? string.Empty;
        public string Caption { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
