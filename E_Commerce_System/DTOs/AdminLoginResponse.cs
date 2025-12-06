namespace E_Commerce_System.DTOs
{
    public class AdminLoginResponse
    {
        public bool Success {  get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
