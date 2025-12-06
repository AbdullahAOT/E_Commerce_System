using E_Commerce_System.Models;

namespace E_Commerce_System.ViewModels
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
