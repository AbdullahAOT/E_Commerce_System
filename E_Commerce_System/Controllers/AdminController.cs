using E_Commerce_System.DTOs;
using E_Commerce_System.Models;
using E_Commerce_System.ViewModels.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace E_Commerce_System.Controllers
{
    public class AdminController : Controller
    {
        public readonly IHttpClientFactory _httpClientFactory;
        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new AdminLoginViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if(string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                model.ErrorMessage = "Email and Password are required";
                return View(model);
            }
            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);
            var loginRequest = new AdminLoginRequest
            {
                Email = model.Email,
                Password=model.Password
            };
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsJsonAsync("api/AdminAuthorization/login", loginRequest);
            }
            catch (Exception)
            {
                model.ErrorMessage = "Unfortunately, we couldn't connect to the server";
                return View(model);
            }
            if (!response.IsSuccessStatusCode)
            {
                model.ErrorMessage = "Invalid Email or Password";
                return View(model);
            }
            var loginResponse = await response.Content.ReadFromJsonAsync<AdminLoginResponse>();
            if(loginResponse == null || !loginResponse.Success)
            {
                model.ErrorMessage = "Invalid Email or Password";
                return View(model);
            }
            HttpContext.Session.SetInt32("AdminId", loginResponse.Id);
            HttpContext.Session.SetString("AdminName", loginResponse.Name);
            HttpContext.Session.SetString("AdminEmail", loginResponse.Email);
            return RedirectToAction("Dashboard");
        }
        public IActionResult Dashboard()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if(adminId == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Customers(int page = 1, int? searchId = null)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            const int pageSize = 10;

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            ViewBag.SearchId = searchId;

            if (searchId.HasValue)
            {
                var response = await client.GetAsync($"api/Customers/{searchId.Value}");

                if (!response.IsSuccessStatusCode)
                {
                    var emptyVm = new CustomerListViewModel
                    {
                        Customers = new List<Customer>(),
                        PageNumber = 1,
                        PageSize = pageSize,
                        TotalCount = 0,
                        TotalPages = 0
                    };

                    return View(emptyVm);
                }

                var customer = await response.Content.ReadFromJsonAsync<Customer>();

                var list = new List<Customer>();
                if (customer != null)
                {
                    list.Add(customer);
                }

                var singleVm = new CustomerListViewModel
                {
                    Customers = list,
                    PageNumber = 1,
                    PageSize = pageSize,
                    TotalCount = list.Count,
                    TotalPages = 1
                };

                return View(singleVm);
            }

            var allResponse = await client.GetAsync("api/Customers");
            if (!allResponse.IsSuccessStatusCode)
            {
                return View(new CustomerListViewModel());
            }

            var allCustomers = await allResponse.Content.ReadFromJsonAsync<List<Customer>>()
                               ?? new List<Customer>();

            var totalCount = allCustomers.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pageItems = allCustomers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new CustomerListViewModel
            {
                Customers = pageItems,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            var response = await client.GetAsync($"api/Customers/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            if (customer == null)
            {
                return NotFound();
            }

            var vm = new CustomerEditViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Password = customer.Password,
                Status = customer.Status,
                Gender = customer.Gender,
                Date_Of_Birth = customer.Date_Of_Birth
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditCustomer(CustomerEditViewModel model)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            var existingResponse = await client.GetAsync($"api/Customers/{model.Id}");
            if (!existingResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var existingCustomer = await existingResponse.Content.ReadFromJsonAsync<Customer>();
            if (existingCustomer == null)
            {
                return NotFound();
            }

            existingCustomer.Name = model.Name;
            existingCustomer.Email = model.Email;
            existingCustomer.Phone = model.Phone;
            existingCustomer.Password = model.Password;
            existingCustomer.Status = model.Status;
            existingCustomer.Gender = model.Gender;
            existingCustomer.Date_Of_Birth = model.Date_Of_Birth;

            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await model.PhotoFile.CopyToAsync(ms);
                    existingCustomer.Photo = ms.ToArray();
                }
            }

            existingCustomer.Update_DateTime_UTC = DateTime.UtcNow;

            var putResponse = await client.PutAsJsonAsync($"api/Customers/{existingCustomer.Id}", existingCustomer);

            if (!putResponse.IsSuccessStatusCode)
            {
                return View(model);
            }

            return RedirectToAction("Customers");
        }

        public async Task<IActionResult> Products(int page = 1)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
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

        [HttpGet]
        public IActionResult CreateProduct()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            return View(new Product());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product model, IFormFile? PhotoFile)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await PhotoFile.CopyToAsync(ms);
                model.Photo = ms.ToArray();
                model.PhotoContentType = PhotoFile.ContentType;
            }

            model.Server_DateTime = DateTime.Now;
            model.DateTime_UTC = DateTime.UtcNow;
            model.Update_DateTime_UTC = null;

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            var response = await client.PostAsJsonAsync("api/Products", model);

            if (!response.IsSuccessStatusCode)
            {
                return View(model);
            }

            return RedirectToAction("Products");
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            var response = await client.GetAsync($"api/Products/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var product = await response.Content.ReadFromJsonAsync<Product>();
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product model, IFormFile? PhotoFile)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            var existingResponse = await client.GetAsync($"api/Products/{model.Id}");
            if (!existingResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var existingProduct = await existingResponse.Content.ReadFromJsonAsync<Product>();
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = model.Name;
            existingProduct.Description = model.Description;
            existingProduct.Status = model.Status;
            existingProduct.Amount = model.Amount;
            existingProduct.Currency = model.Currency;

            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await PhotoFile.CopyToAsync(ms);
                existingProduct.Photo = ms.ToArray();
                existingProduct.PhotoContentType = PhotoFile.ContentType;
            }

            existingProduct.Update_DateTime_UTC = DateTime.UtcNow;

            var putResponse = await client.PutAsJsonAsync($"api/Products/{existingProduct.Id}", existingProduct);
            if (!putResponse.IsSuccessStatusCode)
            {
                return View(model);
            }

            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            var response = await client.DeleteAsync($"api/Products/{id}");

            return RedirectToAction("Products");
        }



        public async Task<IActionResult> Orders(int page = 1, int? customerId = null)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
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
                response = await client.GetAsync("api/Orders");
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

            if (customerId.HasValue)
            {
                allOrders = allOrders
                    .Where(o => o.CustomerId == customerId.Value)
                    .ToList();
            }

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

            ViewBag.CustomerIdFilter = customerId;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomerPermanently(int id, int page = 1)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            client.BaseAddress = new Uri(baseUrl);

            HttpResponseMessage response;
            try
            {
                response = await client.DeleteAsync($"api/Customers/{id}");
            }
            catch
            {
                return RedirectToAction("Customers", new { page });
            }

            return RedirectToAction("Customers", new { page });
        }


    }
}
