using E_Commerce_System.Models;

namespace E_Commerce_System.ViewModels
{
    public class CustomerListViewModel
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
