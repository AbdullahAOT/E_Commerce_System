using E_Commerce_System.Data;
using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_System.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]      // → /api/products
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest("Product data is required!");
            }

            product.Server_DateTime = DateTime.Now;
            product.DateTime_UTC = DateTime.UtcNow;
            product.Update_DateTime_UTC = null;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product updatedProduct)
        {
            if (id != updatedProduct.Id && updatedProduct.Id != 0)
            {
                return BadRequest("The Id in the URL and the Id in the JSON body don't match");
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Status = updatedProduct.Status;
            existingProduct.Amount = updatedProduct.Amount;
            existingProduct.Currency = updatedProduct.Currency;

            // If client included photo bytes, update them. Otherwise keep existing photo.
            if (updatedProduct.Photo != null && updatedProduct.Photo.Length > 0)
            {
                existingProduct.Photo = updatedProduct.Photo;
                existingProduct.PhotoContentType = updatedProduct.PhotoContentType;
            }

            existingProduct.Update_DateTime_UTC = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

