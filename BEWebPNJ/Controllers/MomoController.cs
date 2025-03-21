using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using BEWebPNJ.Models;

namespace BEWebPNJ.Controllers
{
    [Route("api/momo")]
    [ApiController]
    public class MoMoController : ControllerBase
    {
        private const string partnerCode = "YOUR_PARTNER_CODE";
        private const string accessKey = "YOUR_ACCESS_KEY";
        private const string secretKey = "YOUR_SECRET_KEY";
        private const string momoEndpoint = "https://test-payment.momo.vn/v2/gateway/api/create";

        [HttpPost("payment")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] Order order)
        {
            string requestId = Guid.NewGuid().ToString();
            string orderId = requestId;
            string orderInfo = $"Thanh toán đơn hàng {orderId}";
            string returnUrl = "https://your-website.com/payment-success";
            string notifyUrl = "https://your-api.com/api/momo/callback"; // URL nhận callback từ MoMo
            long amount = (long)order.totalAmount;

            var rawData = $"accessKey={accessKey}&amount={amount}&extraData=&ipnUrl={notifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType=captureWallet";

            string signature = ComputeHmacSHA256(rawData, secretKey);

            var requestBody = new
            {
                partnerCode,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                requestType = "captureWallet",
                extraData = "",
                lang = "vi",
                signature
            };

            using var client = new HttpClient();
            var response = await client.PostAsync(momoEndpoint, new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);

            return Ok(new { url = responseObject.payUrl });
        }

        private static string ComputeHmacSHA256(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }


    }
}


