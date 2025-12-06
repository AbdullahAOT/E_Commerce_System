using E_Commerce_System.Models;

namespace E_Commerce_System.ViewModels.Admin
{
    public class CustomerListViewModel
    {
        public List<E_Commerce_System.Models.Customer> Customers { get; set; } 
        = new List<E_Commerce_System.Models.Customer>();

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
