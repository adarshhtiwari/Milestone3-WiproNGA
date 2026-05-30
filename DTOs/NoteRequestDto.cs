using System.ComponentModel.DataAnnotations;

namespace SecureNoteTakingApi.DTOs
{
    public class NoteRequestDto
    {
        //Title of the note 
        [Required(ErrorMessage = "Title is required.")]
        [MinLength(1, ErrorMessage = "Title cannot be empty.")]
        public string Title { get; set; } = string.Empty;

        //Content/body of the note
        [Required(ErrorMessage = "Content is required.")]
        [MinLength(1, ErrorMessage = "Content cannot be empty.")]
        public string Content { get; set; } = string.Empty;
    }
}