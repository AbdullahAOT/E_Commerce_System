
using E_Commerce_System.Data;
using E_Commerce_System.DTOs;
using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace E_Commerce_System.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerAuthorizationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CustomerAuthorizationController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(CustomerLoginRequest request)
        {
            if(request==null || 
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and Password are required");
            }
            var normalizedEmail = request.Email.Trim().ToLower();
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c =>
                c.Email.ToLower() == normalizedEmail &&
                c.Password == request.Password
                );
            if(customer == null)
            {
                return Unauthorized(new
                {
                    success=false,
                    message="Invalid Email or Password"
                });
            }
            if(customer.Status != CustomerStatus.Active)
            {
                var statusMessage = customer.Status switch
                {
                    CustomerStatus.Inactive => "Your account is Inactive",
                    CustomerStatus.Expired => "Your account is Expired",
                    CustomerStatus.Deleted => "Your account is Deleted",
                    _ => "Your account is not active"
                };
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    success = false,
                    message = statusMessage
                });
            }
            customer.Last_Login_DateTime_UTC = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                id=customer.Id,
                name=customer.Name,
                email=customer.Email,
                status=customer.Status,
                lastLoginUtc=customer.Last_Login_DateTime_UTC
            });
        }
    }
}
