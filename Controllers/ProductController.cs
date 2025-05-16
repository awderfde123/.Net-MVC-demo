using demo.Models;
using demo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

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
        public ActionResult<IEnumerable<Product>> Index()
        {
            var products = _service.GetProducts();
            return View(products);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Product>> Detail(int? id)
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
        public ActionResult<IEnumerable<Product>> Detail(Product product, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            if (imageFile != null && imageFile.Length > 0)
            {
                if (product.ImageUrl != null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", product.ImageUrl);
                    System.IO.File.Delete(oldPath);
                }
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                product.ImageUrl = fileName;
            }
            if (product.Id == 0)
            {
                _service.AddProduct(product);
            }
            else
            {
                _service.UpdateProduct(product);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult<IEnumerable<Product>> Delete(int id)
        {
            _service.DeleteProduct(id);
            return RedirectToAction("Index");
        }

    }
}
