using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using System.Collections.Generic;
using BEWebPNJ.Models;

[Route("api/paypal")]
[ApiController]
public class PayPalController : ControllerBase
{
    private readonly string clientId = "AX1gXAjRmWM1CcebUFTGLPJWmN4P_s90iCxQbyZDRUeqYjcho9MoIUL4liON0yeYm9ZVcLmuN3s5fnYK";
    private readonly string clientSecret = "ECQNuFK4Jb2CBsqnu9aVZZoGg1Hu1DRSyHXMJF0BpU3V7o6GvJ7CRiu998anqgSeG_fSmy6lTvLVLCNg";

    private APIContext GetAPIContext()
    {
        var config = new Dictionary<string, string>
        {
            { "mode", "sandbox" } // Chuyển thành "live" nếu deploy
        };

        string accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
        return new APIContext(accessToken) { Config = config };
    }

    [HttpPost("create-payment")]
    public IActionResult CreatePayment([FromBody] BEWebPNJ.Models.Order order)
    {
        var apiContext = GetAPIContext();
        var payment = new Payment
        {
            intent = "sale",
            payer = new Payer { payment_method = "paypal" },
            transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount { total = order.totalAmount.ToString(), currency = "USD" },
                    description = "Thanh toán đơn hàng qua PayPal"
                }
            },
            redirect_urls = new RedirectUrls
            {
                return_url = "http://localhost:5173/payment-success",
                cancel_url = "http://localhost:5173/payment-cancel"
            }
        };

        var createdPayment = payment.Create(apiContext);
        var approvalUrl = createdPayment.links.Find(link => link.rel == "approval_url").href;

        return Ok(new { url = approvalUrl });
    }
}
