using QE180082_Ass1_Product.Models.DTO;
using QE180082_Ass1_Product.Models.Entities;
using QE180082_Ass1_Product.Repositories;

namespace QE180082_Ass1_Product.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAuthService _authService;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, IProductRepository productRepository, IAuthService authService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _authService = authService;
        }

        public async Task<OrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request)
        {
            // Get cart items
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);
            if (cartItems.Count == 0)
            {
                throw new Exception("Cart is empty");
            }

            // Calculate total and validate stock
            int totalAmount = 0;
            var orderItems = new List<OrderItemEntity>();

            foreach (var cartItem in cartItems)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product {cartItem.ProductId} not found");
                }

                //if (product.Stock < cartItem.Quantity)
                //{
                //    throw new Exception($"Insufficient stock for product {product.Name}");
                //}

                totalAmount += product.Price * cartItem.Quantity;

                orderItems.Add(new OrderItemEntity
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = product.Price
                });

                // Update stock
                //product.Stock -= cartItem.Quantity;
                await _productRepository.UpdateAsync(userId,product);
            }

            // Create order
            var order = new OrderEntity
            {
                UserId = userId,
                TotalAmount = totalAmount,
                Status = "pending",
                PaymentStatus = request.PaymentMethod == "COD" ? "pending" : "pending",
                PaymentMethod = request.PaymentMethod,
                ShippingAddress = request.ShippingAddress,
                Phone = request.Phone,
                Notes = request.Notes,
                OrderItems = orderItems
            };

            order = await _orderRepository.CreateAsync(order);

            // Clear cart
            await _cartRepository.ClearCartAsync(userId);

            return MapToOrderResponse(order);
        }

        public async Task<OrderResponse> GetOrderByIdAsync(int userId, int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.UserId != userId)
            {
                throw new Exception("Order not found");
            }

            return MapToOrderResponse(order);
        }

        public async Task<List<OrderResponse>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return orders.Select(MapToOrderResponse).ToList();
        }

        public async Task<OrderResponse> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            order.Status = request.Status;
            order = await _orderRepository.UpdateAsync(order);

            return MapToOrderResponse(order);
        }

        public async Task<OrderResponse> UpdatePaymentStatusAsync(int orderId, UpdatePaymentStatusRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            order.PaymentStatus = request.PaymentStatus;

            // Ensure CreatedAt is UTC
            if (order.CreatedAt.Kind == DateTimeKind.Unspecified)
            {
                order.CreatedAt = DateTime.SpecifyKind(order.CreatedAt, DateTimeKind.Utc);
            }

            order = await _orderRepository.UpdateAsync(order);

            return MapToOrderResponse(order);
        }

        public async Task<List<OrderResponse>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Select(MapToOrderResponse).ToList();
        }

        private OrderResponse MapToOrderResponse(OrderEntity order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod ?? "COD",
                ShippingAddress = order.ShippingAddress,
                Phone = order.Phone,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemResponse
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductImage = oi.Product.Image,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    Subtotal = oi.Price * oi.Quantity
                }).ToList()
            };
        }
    }
}
