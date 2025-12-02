
using System.ComponentModel.DataAnnotations.Schema;


namespace E_Commerce_System.Models
{
    public enum ProductStatus
    {
        Active=1,
        Inactive=2,
        Expired=3,
        Deleted= 4
    }
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public ProductStatus Status { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}
