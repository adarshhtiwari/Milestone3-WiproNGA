using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using SecureNoteTakingApi.Data;
using SecureNoteTakingApi.DTOs;
using SecureNoteTakingApi.Models;
using SecureNoteTakingApi.Services;

namespace SecureNoteTakingApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        //Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            //Validate request body (username/password)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check that username is unique in the database
            bool usernameExists = await _context.Users
                .AnyAsync(u => u.Username == dto.Username);

            if (usernameExists)
                return Conflict(new { message = "Username already exists. Please choose a different one." });

            //Hash the password securely before storing in the database 
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create new user entity and save to database
            var newUser = new User
            {
                Username = dto.Username,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Return success message with the new user's information
            return Ok(new { message = "User registered successfully. Please log in." });
        }

        //Login with username and password
      
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            //Validate request body
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find user in database by username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            //Verify password against stored hash
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password." });

            //Generate JWT token for authenticated user
            string token = _jwtService.GenerateToken(user);

            // Return token + expiry + user info.
            return Ok(new
            {
                token = token,
                expires_in = 3600,
                user = new { username = user.Username }
            });
        }
    }
}