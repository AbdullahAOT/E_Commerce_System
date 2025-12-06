namespace E_Commerce_System.ViewModels.Admin
{
    public class AdminLoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
