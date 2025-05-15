using Microsoft.AspNetCore.Mvc;
using demo.Services;
using demo.Models;
using Microsoft.AspNetCore.Authorization;

namespace demo.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }
        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            var products = _service.GetProducts();
            return View(products);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Detail(int? id)
        {
            if (id == null)
                return View(new Product());

            var product = _service.GetProductById(id.Value);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Detail(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            if (product.Id == 0)
                _service.AddProduct(product); 
            else
                _service.UpdateProduct(product); 

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int id)
        {
            _service.DeleteProduct(id);
            return RedirectToAction("Index");
        }

    }
}
