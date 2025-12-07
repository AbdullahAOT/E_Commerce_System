
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(CustomerRegisterRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Name, Email and Password are required.");
            }

            var normalizedEmail = request.Email.Trim().ToLower();

            var emailExists = await _context.Customers
                .AnyAsync(c => c.Email.ToLower() == normalizedEmail);

            if (emailExists)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "This email is already registered."
                });
            }

            var newCustomer = new Customer
            {
                Name = request.Name,
                Email = normalizedEmail,
                Phone = request.Phone,
                Password = request.Password,
                Gender = request.Gender,
                Date_Of_Birth = request.DateOfBirth,

                Status = CustomerStatus.Active,
                Server_DateTime = DateTime.Now,
                DateTime_UTC = DateTime.UtcNow,
                Update_DateTime_UTC = null,
                Last_Login_DateTime_UTC = null,
                Photo = null
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                id = newCustomer.Id,
                name = newCustomer.Name,
                email = newCustomer.Email,
                status = newCustomer.Status,
                lastLoginUtc = newCustomer.Last_Login_DateTime_UTC
            });
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
