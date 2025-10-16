using QE180082_Ass1_Product.Models.Entities;

namespace QE180082_Ass1_Product.Repositories
{
    public interface ICartRepository
    {
        Task<List<CartItemEntity>> GetCartItemsByUserIdAsync(int userId);
        Task<CartItemEntity?> GetCartItemAsync(int userId, int productId);
        Task<CartItemEntity?> GetCartItemByIdAsync(int id);
        Task<CartItemEntity> AddCartItemAsync(CartItemEntity cartItem);
        Task<CartItemEntity> UpdateCartItemAsync(CartItemEntity cartItem);
        Task DeleteCartItemAsync(int id);
        Task ClearCartAsync(int userId);
    }
}
