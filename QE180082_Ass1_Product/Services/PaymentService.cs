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
                    $"http://localhost:3000//cancel",
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


                //// Create a temporary subscription payment record   
                //var subcriptionPayment = new SubcriptionPaymentEntity
                //{
                //    SubcriptionId = Guid.NewGuid().ToString(),
                //    UserId = userId,
                //    Amount = plan.Price,
                //    Currency = "VND",
                //    PaymentMethod = "PayOS",
                //    PaymentStatus = PaymentStatusEnum.PENDING,
                //    TransactionReference = $"SUB-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                //    ExpiryDate = DateTime.UtcNow.AddMonths(1),
                //    InvoiceUrl = null,
                //    Note = $"Payment for plan {plan.Name}",
                //    OrderCode = payOSResponse.orderCode,
                //};

                //await paymentRepository.CreatePaymentAsync(subcriptionPayment);

                //var userSubcription = new UserSubcriptionEntity
                //{
                //    SubcriptionId = subcriptionPayment.SubcriptionId,
                //    UserId = userId,
                //    PlanId = plan.PlanId,
                //    IsActive = false, // Will be activated upon successful payment
                //    CreatedDate = DateTime.UtcNow,
                //    UpdatedDate = DateTime.UtcNow
                //};

                //await paymentRepository.CreateUserSubscriptionsAsync(userSubcription);

                //await paymentRepository.SaveChangesAsync();

                //   await transaction.CommitAsync();

                return paymentResponse;
            }
            catch (Exception ex)
            {
                //  await transaction.RollbackAsync();
                return null;
            }
        }

        //public bool HandleWebhookFromPayOSAsync(WebhookType payload)
        //{
        //    var transaction = applicationDbContext.Database.BeginTransaction();
        //    try
        //    {
        //        //WebhookData paymentData = payOS.verifyPaymentWebhookData(payload);
        //        //var currentPayment = paymentRepository.GetPaymentByOrderAsync(payload.data.orderCode);
        //        //var userSubcription = paymentRepository.GetUserSubcriptionBySubciptionId(currentPayment!.SubcriptionId!);

        //        bool validFlag = false;

        //        var expiryDateBaseOnPlan = DateTime.UtcNow;

        //        //if (createPaymentRequest.planId == 1)
        //        //{
        //        //    expiryDateBaseOnPlan = expiryDateBaseOnPlan.AddMonths(1);
        //        //}
        //        //else if (createPaymentRequest.planId == 2)
        //        //{
        //        //    expiryDateBaseOnPlan = expiryDateBaseOnPlan.AddMonths(12);
        //        //}
        //        //else if (createPaymentRequest.planId == 3)
        //        //{
        //        //    expiryDateBaseOnPlan = expiryDateBaseOnPlan.AddYears(50);
        //        //}

        //        //if (paymentData.desc == "success")
        //        //{
        //        //    //Update payment status to COMPLETED
        //        //    currentPayment!.PaymentStatus = PaymentStatusEnum.COMPLETED;
        //        //    currentPayment.PaymentDate = DateTime.UtcNow;
        //        //    paymentRepository.UpdatePayment(currentPayment);

        //        //    //Activate user subscription
        //        //    userSubcription!.IsActive = true;
        //        //    userSubcription.StartDate = DateTime.UtcNow;
        //        //    userSubcription.EndDate = DateTime.UtcNow.AddMonths(1);
        //        //    paymentRepository.UpdateUserSubscription(userSubcription);

        //        //    //paymentRepository.SaveChangesAsync();
        //        //    applicationDbContext.SaveChanges();
        //        //    payOSHubContext.Clients.User(userSubcription.UserId!).SendAsync("ReceivePaymentStatus", new
        //        //    {
        //        //        OrderCode = currentPayment.OrderCode,
        //        //        PaymentStatus = currentPayment.PaymentStatus.ToString(),
        //        //        PaymentDate = currentPayment.PaymentDate,
        //        //        ExpiryDate = userSubcription.EndDate,
        //        //    });

        //        //    validFlag = true;
        //        //}
        //        //else
        //        //{
        //        //    //Update payment status to FAILED
        //        //    currentPayment!.PaymentStatus = PaymentStatusEnum.FAILED;
        //        //    currentPayment.PaymentDate = DateTime.UtcNow;
        //        //    paymentRepository.UpdatePayment(currentPayment);

        //        //    //  paymentRepository.SaveChangesAsync();
        //        //    applicationDbContext.SaveChanges();
        //        //    validFlag = false;
        //        //}

        //        transaction.Commit();

        //        return validFlag;
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();
        //        return false;
        //    }
        //}


        //public async Task<IActionResult> CancelPaymentAsync(CancelPaymentRequest cancelPaymentRequest)
        //{
        //    var transaction = applicationDbContext.Database.BeginTransaction();
        //    try
        //    {
        //        var currentPayment = paymentRepository.GetPaymentByOrderAsync(cancelPaymentRequest.OrderCode);

        //        if (currentPayment == null)
        //        {
        //            return ResponseExtension.NotFound("Payment not found!");
        //        }

        //        PaymentLinkInformation cancelledPaymentLinkInfo = await payOS.cancelPaymentLink(cancelPaymentRequest.OrderCode, "User cancel payment.");

        //        //Update payment status to CANCELLED
        //        if (cancelledPaymentLinkInfo.status == "CANCELLED")
        //        {
        //            currentPayment!.PaymentStatus = PaymentStatusEnum.CANCEL;
        //            currentPayment.PaymentDate = DateTime.UtcNow;
        //            paymentRepository.UpdatePayment(currentPayment);
        //            await paymentRepository.SaveChangesAsync();
        //            await transaction.CommitAsync();
        //        }
        //        else
        //        {
        //            ResponseExtension.BadRequest("An error occur with third-party payment system!");
        //        }

        //        return ResponseExtension.OkWithData("Canceled payment successfully!");
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        return ResponseExtension.InternalServerError("An error occure while cancel payment :" + ex.Message);
        //    }
        //}

    }
}
