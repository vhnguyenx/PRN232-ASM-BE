using QE180082_Ass1_Product.Models.DTO;

namespace QE180082_Ass1_Product.Services
{
    public interface ICartService
    {
        Task<CartResponse> GetCartAsync(int userId);
        Task<CartItemResponse> AddToCartAsync(int userId, AddToCartRequest request);
        Task<CartItemResponse> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemRequest request);
        Task DeleteCartItemAsync(int userId, int cartItemId);
        Task ClearCartAsync(int userId);
    }
}
