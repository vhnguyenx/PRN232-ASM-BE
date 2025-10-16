namespace QE180082_Ass1_Product.Models.DTO
{
    public class CreateOrderRequest
    {
        public required string ShippingAddress { get; set; }
        public required string Phone { get; set; }
        public string? Notes { get; set; }
        public string PaymentMethod { get; set; } = "COD";
    }

    public class OrderItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class OrderResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public string? Phone { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
    }

    public class UpdateOrderStatusRequest
    {
        public required string Status { get; set; }
    }

    public class UpdatePaymentStatusRequest
    {
        public required string PaymentStatus { get; set; }
    }
}
