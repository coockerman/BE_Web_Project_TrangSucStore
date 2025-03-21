using PayPal.Api;
using RestSharp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using BEWebPNJ.Models;

namespace BEWebPNJ.Services
{
    public class PaypalService
    {
        private readonly string clientId = "AX1gXAjRmWM1CcebUFTGLPJWmN4P_s90iCxQbyZDRUeqYjcho9MoIUL4liON0yeYm9ZVcLmuN3s5fnYK";
        private readonly string clientSecret = "ECQNuFK4Jb2CBsqnu9aVZZoGg1Hu1DRSyHXMJF0BpU3V7o6GvJ7CRiu998anqgSeG_fSmy6lTvLVLCNg";
        private readonly string exchangeApiKey = "6eadce22f52f471cabb233c4756df396"; // Thay API Key của bạn

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
            {
                { "mode", "sandbox" } // Đổi thành "live" khi deploy
            };

            string accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            return new APIContext(accessToken) { Config = config };
        }

        // Lấy tỷ giá USD -> VND từ API Currency Freaks
        private decimal GetExchangeRateVNDToUSD()
        {
            string url = $"https://api.currencyfreaks.com/v2.0/rates/latest?base=USD&symbols=VND&apikey={exchangeApiKey}";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Get);
            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new Exception("Không thể lấy tỷ giá ngoại tệ");

            JObject jsonData = JObject.Parse(response.Content);
            decimal exchangeRate = decimal.Parse(jsonData["rates"]["VND"].ToString(), CultureInfo.InvariantCulture);

            return exchangeRate;
        }

        // Chuyển đổi số tiền từ VND sang USD
        private decimal ConvertVNDToUSD(decimal vndAmount)
        {
            decimal exchangeRate = GetExchangeRateVNDToUSD(); // Lấy tỷ giá kiểu decimal
            return vndAmount / exchangeRate;
        }


        public (string paymentId, string approvalUrl) CreatePayment(BEWebPNJ.Models.Order order)
        {
            var apiContext = GetAPIContext();

            // Chuyển VND -> USD
            decimal amountInUSD = ConvertVNDToUSD(decimal.Parse(order.totalAmount.ToString()));
            string formattedAmount = amountInUSD.ToString("F2", CultureInfo.InvariantCulture);

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
        {
            new Transaction
            {
                amount = new Amount { total = formattedAmount, currency = "USD" },
                description = "Thanh toán đơn hàng qua PayPal"
            }
        },
                redirect_urls = new RedirectUrls
                {
                    return_url = "http://localhost:5173/user-home/payment-success",
                    cancel_url = "http://localhost:5173/user-home/payment-failed"
                }
            };

            var createdPayment = payment.Create(apiContext);

            // 🔹 Lấy paymentId từ PayPal
            string paymentId = createdPayment.id;

            // 🔹 Lấy approvalUrl để khách hàng thanh toán
            string approvalUrl = createdPayment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;

            return (paymentId, approvalUrl);
        }


        // Xác nhận thanh toán thành công
        public Payment ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = GetAPIContext();
            var payment = new Payment() { id = paymentId };
            var paymentExecution = new PaymentExecution() { payer_id = payerId };

            return payment.Execute(apiContext, paymentExecution);
        }
    }
}
