using E_Commerce_System.Data;
using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_System.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if(customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Customer data is required !");
            }
            customer.Server_DateTime = DateTime.Now;
            customer.DateTime_UTC = DateTime.UtcNow;
            customer.Update_DateTime_UTC = null;
            customer.Last_Login_DateTime_UTC = null;
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.Id },
                customer
            );
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer updatedCustomer)
        {
            if(id != updatedCustomer.Id && updatedCustomer.Id != 0)
            {
                return BadRequest("The Id in the URL and the Id in the JSON body don't match");
            }
            var existingCustomer = await _context.Customers.FindAsync(id);
            if(existingCustomer == null)
            {
                return NotFound();
            }
            existingCustomer.Name = updatedCustomer.Name;
            existingCustomer.Email = updatedCustomer.Email;
            existingCustomer.Phone = updatedCustomer.Phone;
            existingCustomer.Password = updatedCustomer.Password;
            existingCustomer.Status = updatedCustomer.Status;
            existingCustomer.Gender = updatedCustomer.Gender;
            existingCustomer.Date_Of_Birth = updatedCustomer.Date_Of_Birth;
            existingCustomer.Photo = updatedCustomer.Photo;
            existingCustomer.Update_DateTime_UTC = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
