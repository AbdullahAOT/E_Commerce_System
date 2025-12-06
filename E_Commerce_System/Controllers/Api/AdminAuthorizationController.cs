
using E_Commerce_System.Data;
using E_Commerce_System.DTOs;
using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace E_Commerce_System.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAuthorizationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminAuthorizationController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(AdminLoginRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and Password are required.");
            }
            var normalizedEmail = request.Email.Trim().ToLower();
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a =>
                a.Email.ToLower() == normalizedEmail &&
                a.Password == request.Password);
            if (admin == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid email or password."
                });
            }
            return Ok(new
            {
                success = true,
                id=admin.Id,
                name=admin.Name,
                email=admin.Email
            });
        }
    }
}
