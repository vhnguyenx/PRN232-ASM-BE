using QE180082_Ass1_Product.Models.DTO;

namespace QE180082_Ass1_Product.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request);
        Task<OrderResponse> GetOrderByIdAsync(int userId, int orderId);
        Task<List<OrderResponse>> GetUserOrdersAsync(int userId);
        Task<OrderResponse> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request);
        Task<OrderResponse> UpdatePaymentStatusAsync(int orderId, UpdatePaymentStatusRequest request);
        Task<List<OrderResponse>> GetAllOrdersAsync();
    }
}
