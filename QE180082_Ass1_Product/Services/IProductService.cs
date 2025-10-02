using QE180082_Ass1_Product.Models.DTO;
using QE180082_Ass1_Product.Models.Entities;

namespace QE180082_Ass1_Product.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductEntity>> GetAllProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(int id);
        Task<ProductResponse> CreateProductAsync(ProductRequest request);
        Task<ProductResponse?> UpdateProductAsync(int id, ProductRequest request);
        Task<bool> DeleteProductAsync(int id);
    }
}