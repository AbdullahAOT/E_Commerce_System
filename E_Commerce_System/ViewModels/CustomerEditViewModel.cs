using E_Commerce_System.Models;
using Microsoft.AspNetCore.Http;

namespace E_Commerce_System.ViewModels
{
    public class CustomerEditViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!;
        public CustomerStatus Status { get; set; }
        public Gender Gender { get; set; }
        public DateTime Date_Of_Birth { get; set; }
        public IFormFile? PhotoFile { get; set; }
    }
}

