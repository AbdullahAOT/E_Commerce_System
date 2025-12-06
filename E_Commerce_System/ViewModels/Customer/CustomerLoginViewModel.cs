namespace E_Commerce_System.ViewModels.Customer
{
    public class CustomerLoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
