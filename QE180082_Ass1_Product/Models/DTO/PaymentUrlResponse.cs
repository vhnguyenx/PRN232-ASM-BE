using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QE180082_Ass1_Product.Models.DTO
{
    public class PaymentUrlResponse
    {
        public string? CheckoutUrl { get; set; }
        public string? VendorQrCode { get; set; }
        public string? VendorAccountNumber { get; set; }
        public string? PaymentDescription { get; set; }
        public long? OrderCode { get; set; }
        public string? Currency { get; set; }
        public int? PaymentAmount { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
