using RailwayPostgresAPI.Models;
using RailwayPostgresAPI.Repositories.Interfaces;
using RailwayPostgresAPI.Services.Interfaces;

namespace RailwayPostgresAPI.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
            return await _productRepository.CreateAsycn(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
           return await _productRepository.UpdateAsync(id, product);
        }
    }
}
