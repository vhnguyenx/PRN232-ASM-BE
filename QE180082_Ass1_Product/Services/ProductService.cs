using QE180082_Ass1_Product.Models.DTO;
using QE180082_Ass1_Product.Models.Entities;
using QE180082_Ass1_Product.Repositories;

namespace QE180082_Ass1_Product.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync()
        {
            var queryable = await _productRepository.GetAllAsync();
            return queryable.ToList();
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            return MapToResponse(product);
        }

        public async Task<ProductResponse> CreateProductAsync(ProductRequest request)
        {
            var product = new ProductEntity
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Image = request.Image
            };

            var createdProduct = await _productRepository.CreateAsync(product);
            return MapToResponse(createdProduct);
        }

        public async Task<ProductResponse?> UpdateProductAsync(int id, ProductRequest request)
        {
            var product = new ProductEntity
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Image = request.Image
            };

            var updatedProduct = await _productRepository.UpdateAsync(id, product);
            if (updatedProduct == null)
                return null;

            return MapToResponse(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        private static ProductResponse MapToResponse(ProductEntity product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Image = product.Image
            };
        }
    }
}
