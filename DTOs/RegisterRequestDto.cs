using System.ComponentModel.DataAnnotations;

namespace SecureNoteTakingApi.DTOs
{
    public class RegisterRequestDto
    {
        //Username must be at least 3 characters
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
        public string Username { get; set; } = string.Empty;

        //Password must be at least 8 chars, with uppercase, digit, and special char
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;
    }
}