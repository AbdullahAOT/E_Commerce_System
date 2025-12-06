using E_Commerce_System.Models;

namespace E_Commerce_System.DTOs
{
    public class CustomerLoginResponse
    {
        public bool Success { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public CustomerStatus Status { get; set; }
        public DateTime? LastLoginUtc { get; set; }
    }
}
