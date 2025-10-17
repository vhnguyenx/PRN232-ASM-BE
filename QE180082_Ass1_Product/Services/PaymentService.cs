using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using QE180082_Ass1_Product.Models.DTO;
using QE180082_Ass1_Product.Models.Entities;
using QE180082_Ass1_Product.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QE180082_Ass1_Product.Services
{
    public class PaymentService
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IAuthService authService;
        private readonly PayOS payOS;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PaymentService(IOrderRepository orderRepository, IProductRepository productRepository, ICartRepository cartRepository, IAuthService authService, PayOS payOS, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            this.authService = authService;
            this.payOS = payOS;
            this.httpContextAccessor = httpContextAccessor;
        }


        public async Task<PaymentUrlResponse> CreatePaymentRequestAsync(CreateOrderRequest createPaymentRequest, int userId)
        {
            //  var transaction = applicationDbContext.Database.BeginTransaction();
            try
            {
                //var plan = await planFeatureRepostiory.GetPlanByIdAsync(createPaymentRequest.planId);

                //if (plan == null)
                //{
                //    return ResponseExtension.BadRequest("Plan not found.");
                //}

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
                    await _productRepository.UpdateAsync(userId, product);
                }

                // Use current timestamp as order code for uniqueness
                var orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

                // Create order
                var order = new OrderEntity
                {
                    Id = orderCode,
                    UserId = userId,
                    TotalAmount = totalAmount,
                    Status = "pending",
                    PaymentStatus = createPaymentRequest.PaymentMethod == "PayOS" ? "pending" : "pending",
                    PaymentMethod = createPaymentRequest.PaymentMethod,
                    ShippingAddress = createPaymentRequest.ShippingAddress,
                    Phone = createPaymentRequest.Phone,
                    Notes = createPaymentRequest.Notes,
                    OrderItems = orderItems,
                    
                };

                // Create item list based on the plan details
                ItemData item = new ItemData($"Order Product", 1, order.TotalAmount);
                List<ItemData> items = new List<ItemData> { item };

                // Get the current request's base URL
                var request = httpContextAccessor.HttpContext!.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                // Get user profile
                var userProfile = await authService.GetUserByIdAsync(userId);

               // var planDescription = order.Name;

                // Create payment data to send to PayOS
                var paymentData = new PaymentData(
                    orderCode,
                    order.TotalAmount,
                    $"Order Product",
                    items,
                    $"https://prn-232-asm-fe.vercel.app/cancel",
                    $"https://prn-232-asm-fe.vercel.app/payos-success",
                    null, // signature
                    userProfile?.Email ?? "Valued Customer", // buyerName
                    userProfile?.Email, // buyerEmail
                    order?.Phone, // buyerPhone
                    order?.ShippingAddress, // buyerAddress
                    null // expiredAt
                );

                var payOSResponse = await payOS.createPaymentLink(paymentData);

                var paymentResponse = new PaymentUrlResponse
                {
                    CheckoutUrl = payOSResponse.checkoutUrl,
                    VendorQrCode = payOSResponse.qrCode,
                    VendorAccountNumber = payOSResponse.accountNumber,
                    PaymentDescription = payOSResponse.description,
                    OrderCode = payOSResponse.orderCode,
                    Currency = payOSResponse.currency,
                    PaymentAmount = payOSResponse.amount,
                    PaymentStatus = payOSResponse.status,
                };

                order = await _orderRepository.CreateAsync(order);

                // Clear cart
                await _cartRepository.ClearCartAsync(userId);

                return paymentResponse;
            }
            catch (Exception ex)
            {
                //  await transaction.RollbackAsync();
                return null;
            }
        }

  
    }
}
