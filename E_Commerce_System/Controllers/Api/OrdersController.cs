using E_Commerce_System.Data;
using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace E_Commerce_System.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrdersController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .ToListAsync();
            return Ok(orders);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id==id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            if (order == null)
            {
                return BadRequest("Data of the order is mandatory to create a new order");
            }
            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == order.CustomerId);
            if (!customerExists)
            {
                return BadRequest($"Customer with the id \"{order.CustomerId}\" does not exist");
            }
            var productExists = await _context.Products
                .AnyAsync(p => p.Id == order.ProductId);
            if (!productExists)
            {
                return BadRequest($"Product with the id \"{order.ProductId}\" does not exist");
            }
            order.Server_DateTime = DateTime.Now;
            order.DateTime_UTC = DateTime.UtcNow;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = order.Id },
                order
            );
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order updatedOrder)
        {
            if(id != updatedOrder.Id && updatedOrder.Id != 0)
            {
                return BadRequest("URL id and JSON body id don't match");
            }
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }
            if(updatedOrder.CustomerId != 0 && updatedOrder.CustomerId != existingOrder.CustomerId)
            {
                var customerExists= await _context.Customers
                    .AnyAsync(c=> c.Id == updatedOrder.CustomerId);
                if (!customerExists)
                {
                    return BadRequest($"Customer with the id \"{updatedOrder.CustomerId}\" does not exist");
                }
                existingOrder.CustomerId = updatedOrder.CustomerId;
            }
            if(updatedOrder.ProductId !=0 && updatedOrder.ProductId != existingOrder.ProductId)
            {
                var productExists= await _context.Products
                    .AnyAsync(p=> p.Id == updatedOrder.ProductId);
                if (!productExists)
                {
                    return BadRequest($"Product with the id \"{updatedOrder.ProductId}\" does not exist");
                }
                existingOrder.ProductId = updatedOrder.ProductId;
            }
            existingOrder.Total_Amount = updatedOrder.Total_Amount;
            existingOrder.Currency = updatedOrder.Currency;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }
            _context.Orders.Remove(existingOrder);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomer(int customerId)
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            return Ok(orders);
        }

    }
}
