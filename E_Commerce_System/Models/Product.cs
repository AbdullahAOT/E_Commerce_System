using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace E_Commerce_System.Models
{
    public enum ProductStatus
    {
        Active = 1,
        Inactive = 2,
        Expired = 3,
        Deleted = 4
    }
    public class Product
    {
        public int Id { get; set; }
        public DateTime Server_DateTime { get; set; }
        public DateTime DateTime_UTC { get; set; }
        public DateTime? Update_DateTime_UTC { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProductStatus Status { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        // Photo stored as varbinary in DB and serialized over JSON
        [Column(TypeName = "varbinary(max)")]
        public byte[]? Photo { get; set; }

        public string? PhotoContentType { get; set; }

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
