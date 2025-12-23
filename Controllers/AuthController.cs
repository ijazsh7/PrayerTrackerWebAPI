using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PrayerTrackerWebAPI.Models;

namespace PrayerTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PrayerDbContext _context; 
        public AuthController(PrayerDbContext context)
        {
            _context = context;
  
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email || u.Username == model.Username))
                return BadRequest(new { message = "Username or Email already taken" });

            var user = new User
            {
                Username = model.Username,
                Email =    model.Email,
                Password = model.Password,
                Role = "User",
                IsActive = true,                
                CreatedAt= DateTime.Now
            };


            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)       {
            

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Username || u.Username == model.Username);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // Check if the user is active
            if ((bool)!user.IsActive)
                return Unauthorized(new { message = "Your account is deactivated. Please contact support." });            

            // Verify the password matches that of the user password
            bool isPasswordValid = (model.Password == user.Password);

            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid credentials" });


            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);

            return Ok(new { message = "Login successful" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Clear the session when logging out
            HttpContext.Session.Clear();

            return Ok(new { message = "Logout successful" });

        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.UserId, u.Username, u.Email })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("usersession")]
        public IActionResult CheckSession()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                // Return userId of 0 and null username if session is not defined
                return Ok(new { message = "Session not active", userId = 0, userName = (string?)null, userRole = (string?)null });
            }

            return Ok(new { message = "Session active", userId, userName, userRole });
        }

    }



    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }  
    }


    public class LoginModel
    {
        [Required]
        public string Username { get; set; } // Can be username or email

        [Required]
        public string Password { get; set; }
    }



}
