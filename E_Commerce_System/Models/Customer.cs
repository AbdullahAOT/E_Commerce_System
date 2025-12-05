using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_Commerce_System.Models
{
    public enum CustomerStatus
    {
        Active = 1,
        Inactive = 2,
        Expired = 3,
        Deleted = 4
    }
    public enum Gender
    {
        Male = 1,
        Female = 2
    }
    public class Customer
    {
        public int Id { get; set; }
        public DateTime Server_DateTime { get; set; }
        public DateTime DateTime_UTC { get; set; }
        public DateTime? Update_DateTime_UTC { get; set; }
        public DateTime? Last_Login_DateTime_UTC { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        public byte[]? Photo { get; set; }
        [Required]
        public CustomerStatus Status { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public DateTime Date_Of_Birth { get; set; }
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
