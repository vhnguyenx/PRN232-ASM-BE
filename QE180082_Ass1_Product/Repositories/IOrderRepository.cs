using QE180082_Ass1_Product.Models.Entities;

namespace QE180082_Ass1_Product.Repositories
{
    public interface IOrderRepository
    {
        Task<OrderEntity?> GetByIdAsync(int id);
        Task<List<OrderEntity>> GetOrdersByUserIdAsync(int userId);
        Task<OrderEntity> CreateAsync(OrderEntity order);
        Task<OrderEntity> UpdateAsync(OrderEntity order);
        Task<List<OrderEntity>> GetAllOrdersAsync();
    }
}
