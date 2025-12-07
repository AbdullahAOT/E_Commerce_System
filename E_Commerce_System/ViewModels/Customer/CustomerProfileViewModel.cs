using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace E_Commerce_System.ViewModels.Customer
{
    public class CustomerProfileViewModel
    {
        // Display-only
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        // Upload
        public IFormFile? Photo { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
