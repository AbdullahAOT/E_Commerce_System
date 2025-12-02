namespace E_Commerce_System.Models
{
    public enum CustomerStatus
    {
        Active = 1,
        Inactive = 2,
        Expired = 3,
        Deleted = 4
    }
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string phoneNumber { get; set; }
        public CustomerStatus Status { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}
