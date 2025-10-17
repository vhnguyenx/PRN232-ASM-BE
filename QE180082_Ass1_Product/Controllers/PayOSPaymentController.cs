using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using QE180082_Ass1_Product.Models.DTO;
using QE180082_Ass1_Product.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QE180082_Ass1_Product.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOSPaymentController : ControllerBase
    {
        private readonly PaymentService paymentService;

        public PayOSPaymentController(PaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        /// <summary>
        /// Creates a new payment intent based on the specified payment request.
        /// </summary>
        /// <param name="createPaymentRequest">The payment request details used to create the payment intent. Cannot be null.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the payment intent creation operation.</returns>
        [HttpPost]
        public async Task<PaymentUrlResponse> CreatePaymentIntent([FromBody] CreateOrderRequest createPaymentRequest)
        {
            int userId = GetUserId();
            return await paymentService.CreatePaymentRequestAsync(createPaymentRequest, userId);
        }
    }
}
