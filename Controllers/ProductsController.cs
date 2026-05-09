using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RailwayPostgresAPI.Dtos;
using RailwayPostgresAPI.Models;
using RailwayPostgresAPI.Services.Interfaces;

namespace RailwayPostgresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAll()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound($"Product with ID {id} is not found");
                return Ok(product); 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting product {id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto dto)
        {
            try
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price
                };
                var createdProduct = await _productService.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error creating a product");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto dto)
        {
            try
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price
                };
                var updateProduct = await _productService.UpdateProductAsync(id, product);
                if (updateProduct == null)
                    return NotFound($"Product with ID {id} not found!");
                return Ok(updateProduct);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating product {id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _productService.DeleteProductAsync(id);
                if (!deleted)
                    return NotFound($"Product with Id {id} not found");
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
