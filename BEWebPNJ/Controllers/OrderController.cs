using BEWebPNJ.Models;
using BEWebPNJ.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BEWebPNJ.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // ✅ [POST] Tạo đơn hàng mới
        [HttpPost("create")]
        public async Task<ActionResult> CreateOrder([FromBody] Order order)
        {
            if (order.productItems == null || order.productItems.Count == 0)
                return BadRequest(new { message = "Đơn hàng không hợp lệ, không có sản phẩm nào." });

            try
            {
                var orderId = await _orderService.CreateOrder(order);
                return Ok(new { message = "Đơn hàng đã được tạo.", orderId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // ✅ [GET] Lấy thông tin đơn hàng theo ID
        [HttpGet("detail/{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(string orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            return order != null 
                ? Ok(order) 
                : NotFound(new { message = $"Không tìm thấy đơn hàng có ID '{orderId}'." });
        }

        // ✅ [GET] Lấy danh sách tất cả đơn hàng
        [HttpGet("all")]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return orders.Count > 0 
                ? Ok(orders) 
                : NotFound(new { message = "Không có đơn hàng nào trong hệ thống." });
        }

        // ✅ [PATCH] Cập nhật trạng thái đơn hàng
        [HttpPatch("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] string newStatus)
        {
            var result = await _orderService.UpdateOrderStatus(orderId, newStatus);
            return result 
                ? Ok(new { message = "Trạng thái đơn hàng đã được cập nhật." }) 
                : NotFound(new { message = $"Không tìm thấy đơn hàng có ID '{orderId}'." });
        }
        // ✅ [GET] Lấy danh sách đơn hàng theo idUserOrder
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Order>>> GetOrdersByUserId(string userId)
        {
            var orders = await _orderService.GetOrdersByUserId(userId);
            return orders.Count > 0
                ? Ok(orders)
                : NotFound(new { message = $"Không tìm thấy đơn hàng nào của người dùng '{userId}'." });
        }

        // ✅ [DELETE] Xóa đơn hàng theo ID
        [HttpDelete("delete/{orderId}")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            var result = await _orderService.DeleteOrder(orderId);
            return result 
                ? Ok(new { message = "Đơn hàng đã được xóa." }) 
                : NotFound(new { message = $"Không tìm thấy đơn hàng có ID '{orderId}'." });
        }
        // ✅ [POST] Tạo URL thanh toán VNPAY
        [HttpPost("payment")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] Order order)
        {
            if (order == null || order.totalAmount <= 0)
                return BadRequest(new { message = "Đơn hàng không hợp lệ." });

            string orderId = await _orderService.CreateOrder(order);
            if (string.IsNullOrEmpty(orderId)) return StatusCode(500, "Lỗi tạo đơn hàng.");

            // 🔹 Cấu hình thông tin thanh toán VNPAY
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_TmnCode = "RGS2UORK";
            string vnp_HashSecret = "2JD1FWHTS2JY113G4FD8QND68RSL6AB9";

            string vnp_TxnRef = orderId;
            string vnp_OrderInfo = "Thanhtoandonhang" + orderId;
            string vnp_ReturnUrl = "https://192e-171-224-178-91.ngrok-free.app/";
            string vnp_IpAddr = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            int vnp_Amount = (int)(order.totalAmount * 100); // Đơn vị là VND (x100)

            var pay = new SortedDictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_Amount", vnp_Amount.ToString() },
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", vnp_TxnRef },
            { "vnp_OrderInfo", vnp_OrderInfo },
            { "vnp_OrderType", "other" },
            { "vnp_Locale", "vn" },
            { "vnp_ReturnUrl", vnp_ReturnUrl },
            { "vnp_IpAddr", vnp_IpAddr },
            { "vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") }
        };

            // 🔹 Tạo chữ ký HMAC SHA512
            string queryString = string.Join("&", pay.Select(kvp => kvp.Key + "=" + kvp.Value));
            string secureHash = CreateSecureHash(queryString, vnp_HashSecret);
            string paymentUrl = $"{vnp_Url}?{queryString}&vnp_SecureHash={secureHash}";

            return Ok(new { url = paymentUrl });
        }

        // ✅ [GET] Xử lý callback từ VNPAY
        [HttpGet("payment/callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var queryParams = HttpContext.Request.Query
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            if (!queryParams.ContainsKey("vnp_TxnRef") || !queryParams.ContainsKey("vnp_ResponseCode"))
                return BadRequest(new { message = "Thiếu tham số từ VNPAY." });

            string orderId = queryParams["vnp_TxnRef"];
            string responseCode = queryParams["vnp_ResponseCode"];
            string vnp_HashSecret = "2JD1FWHTS2JY113G4FD8QND68RSL6AB9";

            // 🔹 Kiểm tra chữ ký bảo mật
            if (!VerifySignature(queryParams, vnp_HashSecret))
                return BadRequest(new { message = "Chữ ký không hợp lệ." });

            // 🔹 Cập nhật trạng thái đơn hàng
            string newStatus = responseCode == "00" ? "pending" : "process";
            await _orderService.UpdateOrderStatus(orderId, newStatus);

            return Redirect($"http://localhost:5173/authentication?status={newStatus}");
        }

        // 🔹 Hàm tạo chữ ký bảo mật HMAC SHA512
        private static string CreateSecureHash(string rawData, string secret)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        // 🔹 Hàm kiểm tra chữ ký phản hồi từ VNPAY
        private static bool VerifySignature(Dictionary<string, string> values, string secret)
        {
            if (!values.ContainsKey("vnp_SecureHash")) return false;

            string secureHash = values["vnp_SecureHash"];
            values.Remove("vnp_SecureHash");

            var sortedValues = new SortedDictionary<string, string>(values);
            string queryString = string.Join("&", sortedValues.Select(kvp => kvp.Key + "=" + kvp.Value));
            string computedHash = CreateSecureHash(queryString, secret);

            return secureHash.Equals(computedHash, StringComparison.OrdinalIgnoreCase);
        }
        [HttpPost("callback")]
        public async Task<IActionResult> PaymentCallback([FromBody] dynamic response)
        {
            string orderId = response.orderId;
            string requestId = response.requestId;
            string message = response.message;
            string resultCode = response.resultCode;

            if (resultCode == "0")
            {
                await _orderService.UpdateOrderStatus(orderId, "pending");
                return Ok(new { message = "Thanh toán thành công!" });
            }
            else
            {
                await _orderService.UpdateOrderStatus(orderId, "process");
                return BadRequest(new { message = "Thanh toán thất bại!" });
            }
        }

    }
}
