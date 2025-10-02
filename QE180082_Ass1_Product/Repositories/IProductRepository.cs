using QE180082_Ass1_Product.Models.Entities;

namespace QE180082_Ass1_Product.Repositories
{
    public interface IProductRepository
    {
        Task<IQueryable<ProductEntity>> GetAllAsync();
        Task<ProductEntity?> GetByIdAsync(int id);
        Task<ProductEntity> CreateAsync(ProductEntity product);
        Task<ProductEntity?> UpdateAsync(int id, ProductEntity product);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}