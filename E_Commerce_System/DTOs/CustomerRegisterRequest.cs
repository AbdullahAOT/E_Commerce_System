using E_Commerce_System.Models;

namespace E_Commerce_System.DTOs
{
    public class CustomerRegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
