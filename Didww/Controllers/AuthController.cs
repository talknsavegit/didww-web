using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace DIDWW_Api.Controllers
{
    public class UserCredentials
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    [Controller]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            try
            {
                var username = "Admin";
                var token = GenerateToken(username);

                // Pass the token to the view
                ViewData["Token"] = token;

                // Return the Login view
                return View("Login");
            }
            catch (Exception ex)
            {
                // Handle exceptions that may occur during token generation
                // Log the exception for troubleshooting
                Console.WriteLine($"An exception occurred during token generation: {ex.Message}");
                return StatusCode(500, new { Error = "Internal server error", ExceptionMessage = ex.Message });
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token != null)
            {
                HttpContext.Session.Remove("Token");
                return Unauthorized(new { Error = "Logout Successfully" });
            }
            else
            {
                return Unauthorized(new { Error = "UnAuthorized! Please Login First" });
            }
        }

        private string GenerateToken(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return "Invalid username.";
                }
                var time = DateTime.Now.ToString();

                byte[] token = Encoding.UTF8.GetBytes(username + "_" + DateTime.Now.ToString());
                var tokenstr = Convert.ToBase64String(token);
                HttpContext.Session.SetString("Token", tokenstr);
                return tokenstr;
            }
            catch (Exception ex)
            {
                // Handle exceptions that may occur during token generation
                // Log the exception for troubleshooting
                Console.WriteLine($"An exception occurred during token generation: {ex.Message}");
                return "Error generating token";
            }
        }
    }
}
