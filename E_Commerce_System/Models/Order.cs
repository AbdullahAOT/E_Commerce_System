
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; } 
        public Customer? Customer { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime Server_DateTime { get; set; }
        public DateTime DateTime_UTC { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total_Amount { get; set; }
        public string Currency { get; set; }
    }
}
