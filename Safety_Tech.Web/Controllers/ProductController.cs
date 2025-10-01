using Microsoft.AspNetCore.Mvc;
using Safety_Tech.Models;

namespace Safety_Tech.Web.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            // Static data
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 45000 },
                new Product { Id = 2, Name = "Mobile", Category = "Electronics", Price = 25000 },
                new Product { Id = 3, Name = "Shoes", Category = "Fashion", Price = 3000 },
                new Product { Id = 4, Name = "Shirt", Category = "Fashion", Price = 1200 },
                new Product { Id = 5, Name = "Book", Category = "Stationary", Price = 500 }
            };

            return View(products);
        }
    }
}
