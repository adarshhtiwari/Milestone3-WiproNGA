

using System.ComponentModel.DataAnnotations;

namespace SecureNoteTakingApi.DTOs
{
    public class LoginRequestDto
    {
        //Username sent by user for login verification
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        //Password verified against stored hash
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}