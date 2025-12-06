using System.Net.Http.Json;
using E_Commerce_System.DTOs;
using E_Commerce_System.Models;
using E_Commerce_System.ViewModels.Admin;
using E_Commerce_System.ViewModels.Customer; 
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View(new CustomerLoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(CustomerLoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                model.ErrorMessage = "Email and Password are required.";
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            var request = new CustomerLoginRequest
            {
                Email = model.Email,
                Password = model.Password
            };

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsJsonAsync("api/CustomerAuthorization/login", request);
            }
            catch
            {
                model.ErrorMessage = "Could not connect to the server.";
                return View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                model.ErrorMessage = "Invalid email or password.";
                return View(model);
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<CustomerLoginResponse>();
            if (loginResponse == null || !loginResponse.Success)
            {
                model.ErrorMessage = "Invalid email or password.";
                return View(model);
            }
            HttpContext.Session.SetInt32("CustomerId", loginResponse.Id);
            HttpContext.Session.SetString("CustomerName", loginResponse.Name);
            HttpContext.Session.SetString("CustomerEmail", loginResponse.Email);

            return RedirectToAction("Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        public async Task<IActionResult> Dashboard()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login");
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync($"api/Orders/by-customer/{customerId.Value}");
            }
            catch
            {
                ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");
                ViewBag.TotalOrders = 0;
                ViewBag.TotalAmount = 0m;
                return View();
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");
                ViewBag.TotalOrders = 0;
                ViewBag.TotalAmount = 0m;
                return View();
            }

            var orders = await response.Content.ReadFromJsonAsync<List<Order>>() ?? new List<Order>();

            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");
            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalAmount = orders.Sum(o => o.Total_Amount);

            return View();
        }

        public async Task<IActionResult> Products(int page = 1)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login");
            }

            const int pageSize = 10;

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync("api/Products");
            }
            catch
            {
                return View(new ProductListViewModel());
            }

            if (!response.IsSuccessStatusCode)
            {
                return View(new ProductListViewModel());
            }

            var allProducts = await response.Content.ReadFromJsonAsync<List<Product>>() ?? new List<Product>();

            allProducts = allProducts
                .Where(p => p.Status == ProductStatus.Active)
                .ToList();

            var totalCount = allProducts.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pageItems = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new ProductListViewModel
            {
                Products = pageItems,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int productId)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login");
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            var productResponse = await client.GetAsync($"api/Products/{productId}");
            if (!productResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Products");
            }

            var product = await productResponse.Content.ReadFromJsonAsync<Product>();
            if (product == null)
            {
                return RedirectToAction("Products");
            }

            var newOrder = new Order
            {
                CustomerId = customerId.Value,
                ProductId = product.Id,
                Total_Amount = product.Amount,
                Currency = product.Currency
            };

            var orderResponse = await client.PostAsJsonAsync("api/Orders", newOrder);
            if (!orderResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Products");
            }

            return RedirectToAction("MyOrders");
        }

        public async Task<IActionResult> MyOrders(int page = 1)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login");
            }

            const int pageSize = 10;

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync($"api/Orders/by-customer/{customerId.Value}");
            }
            catch
            {
                return View(new OrderListViewModel());
            }

            if (!response.IsSuccessStatusCode)
            {
                return View(new OrderListViewModel());
            }

            var allOrders = await response.Content.ReadFromJsonAsync<List<Order>>() ?? new List<Order>();

            var totalCount = allOrders.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pageItems = allOrders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new OrderListViewModel
            {
                Orders = pageItems,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login");
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            var getResponse = await client.GetAsync($"api/Orders/{id}");
            if (!getResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("MyOrders");
            }

            var order = await getResponse.Content.ReadFromJsonAsync<Order>();
            if (order == null || order.CustomerId != customerId.Value)
            {
                return RedirectToAction("MyOrders");
            }

            await client.DeleteAsync($"api/Orders/{id}");

            return RedirectToAction("MyOrders");
        }
    }
}
