using E_Commerce_System.Models;

namespace E_Commerce_System.ViewModels.Admin
{
    public class OrderListViewModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
