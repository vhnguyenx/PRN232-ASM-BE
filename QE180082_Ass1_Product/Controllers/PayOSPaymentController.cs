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

        ///// <summary>
        ///// Receives and processes a webhook notification from PayOS.
        ///// </summary>
        ///// <remarks>This endpoint does not require authentication. It is intended to be called by PayOS
        ///// to notify the system of payment events. The response object includes a status code and message indicating
        ///// the outcome of processing the webhook.</remarks>
        ///// <param name="payload">The webhook payload received from PayOS containing event data to be processed. Cannot be null.</param>
        ///// <returns>An HTTP 200 OK response containing a result object indicating whether the webhook was processed
        ///// successfully.</returns>
        //[AllowAnonymous]
        //[HttpPost("webhook-payos")]
        //public IActionResult ReceiveWebhookFromPayOS([FromBody] WebhookType payload)
        //{
        //    bool isWebhookValid = _paymentService.HandleWebhookFromPayOSAsync(payload);

        //    if (!isWebhookValid)
        //    {
        //        return Ok(new PayOSWebhookResponse(1, "Error", null));
        //    }

        //    return Ok(new PayOSWebhookResponse(0, "Success", null));
        //}

        ///// <summary>
        ///// Cancels a payment based on the specified cancellation request.
        ///// </summary>
        ///// <remarks>This endpoint requires the caller to be authenticated. The cancellation is performed
        ///// for the current user as determined by the authentication context.</remarks>
        ///// <param name="cancelPaymentRequest">An object containing the details of the payment to cancel. Must include valid payment identification and
        ///// cancellation information.</param>
        ///// <returns>An <see cref="IActionResult"/> indicating the result of the cancellation operation. Returns a success
        ///// response if the payment is canceled; otherwise, returns an error response describing the failure.</returns>
        //[HttpPost("cancel-payment")]
        //public async Task<IActionResult> CancelPayment([FromBody] CancelPaymentRequest cancelPaymentRequest)
        //{
        //    var userId = _currentUserService.GetUserId();
        //    return await _paymentService.CancelPaymentAsync(cancelPaymentRequest);
        //}
    }
}
