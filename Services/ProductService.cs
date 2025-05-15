using demo.Models;
using demo.Repositories;
using System.Collections.Generic;

namespace demo.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repo;

        public ProductService(ProductRepository repo)
        {
            _repo = repo;
        }

        public List<Product> GetProducts()
        {
            return _repo.GetAll();
        }

        public Product? GetProductById(int id)
        {
            return _repo.GetById(id);
        }

        public void AddProduct(Product product)
        {
            _repo.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _repo.Update(product);
        }

        public void DeleteProduct(int id)
        {
            _repo.Delete(id);
        }
    }
}
